using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Akka.Actor;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Newtonsoft.Json;
using RhyoliteOrbit.Actors.MessageModels;
using RhyoliteOrbit.CloudinaryApi;
using RhyoliteOrbit.RabbitMq;
using RhyoliteOrbit.ViewRenderer;
using Syncfusion.HtmlConverter;
using Syncfusion.Pdf;

namespace RhyoliteOrbit.Actors;

public class StudentBillActor: ReceiveActor
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<StudentBillActor> _logger;
    
    public StudentBillActor(IServiceProvider serviceProvider)
    {
        // Store the root service provider, not scoped services
        _serviceProvider = serviceProvider;
        _logger = serviceProvider.GetService<ILogger<StudentBillActor>>() ?? NullLogger<StudentBillActor>.Instance;
        
        ReceiveAsync<List<StudentBillDto>>(async a => await HandleMessage(a));
    }
    
    
    
    private async Task HandleMessage(List<StudentBillDto> msgList)
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            try
            {
                var viewRenderService = scope.ServiceProvider.GetRequiredService<IViewRenderService>();
                var webHostEnvironment = scope.ServiceProvider.GetRequiredService<IWebHostEnvironment>();
                var fileUploadApi = scope.ServiceProvider.GetRequiredService<IFileUploadApi>();
                var rabbitMqClient = scope.ServiceProvider.GetRequiredService<IRabbitMqClient>();
                var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();
                
                var studentBillReportUpdatesQueue = configuration["RabbitMqBroker:StudentBillReportUpdatesQueue"];
                var topicExchangeType = configuration["RabbitMqBroker:ExchangeTypes:AmqTopic"];

                List<MemoryStream> streamArray = new List<MemoryStream>();
                List<object> studentBillReportList = new List<object>();
                
                string reportsFolder = Path.Combine(webHostEnvironment.WebRootPath, "Reports");
                Directory.CreateDirectory(reportsFolder);

                string blinkEnginePath = RuntimeInformation.IsOSPlatform(OSPlatform.OSX)
                    ? Path.Combine(webHostEnvironment.WebRootPath, "PdfEngine", "mac")
                    : Path.Combine(webHostEnvironment.WebRootPath, "PdfEngine", "linux");

                _logger.LogInformation("Blink engine path: " + blinkEnginePath);

                var htmlConverter = new HtmlToPdfConverter(HtmlRenderingEngine.Blink);
                var blinkConverterSettings = new BlinkConverterSettings
                {
                    BlinkPath = blinkEnginePath,
                    Orientation = PdfPageOrientation.Portrait,
                    EnableJavaScript = true,
                    AdditionalDelay = 3000
                };
                blinkConverterSettings.CommandLineArguments.Add("--no-sandbox");
                blinkConverterSettings.CommandLineArguments.Add("--disable-setuid-sandbox");

                htmlConverter.ConverterSettings = blinkConverterSettings;

                foreach (var msg in msgList)
                {
                    var reportHtml = await viewRenderService.RenderToStringAsync("Home/StudentBillReport", msg);

                    _logger.LogInformation($"Successfully generated html for student_bill_report for student {msg.StudentInfo.Id}");

                    var billStream = new MemoryStream();
                    using (var document1 = htmlConverter.Convert(reportHtml, string.Empty))
                    {
                        document1.Save(billStream);
                        document1.Close(false);
                    }

                    // Save/write/upload as usual (after reset)
                    billStream.Position = 0;
                    string individualReportPath = Path.Combine(reportsFolder, $"{msg.StudentInfo.Id}.pdf");
                    using (var fileStream = new FileStream(individualReportPath, FileMode.Create, FileAccess.Write))
                    {
                        await billStream.CopyToAsync(fileStream);
                    }

                    // >>>> CREATE a CLONE of billStream for merging BEFORE upload (which might dispose it) <<<<
                    billStream.Position = 0;
                    var mergeCopy = new MemoryStream();
                    await billStream.CopyToAsync(mergeCopy);
                    mergeCopy.Position = 0;
                    streamArray.Add(mergeCopy);

                    billStream.Position = 0;
                    var uploadedFileUrl = await fileUploadApi.UploadFileStream(billStream, msg.StudentInfo.Id.ToString(), ".pdf");
                    _logger.LogInformation($"Uploaded student bill PDF: {uploadedFileUrl}");

                    // Now dispose billStream after use
                    await billStream.DisposeAsync();
                    
                    studentBillReportList.Add(new {
                        StudentId = msg.StudentInfo.Id,
                        BillReportUrl = uploadedFileUrl
                    });

                }

                // Merge all CLONED streams (mergeCopy in streamArray)
                var finalDoc = new PdfDocument { PageSettings = { Size = PdfPageSize.A4 } };
                PdfDocumentBase.Merge(finalDoc, streamArray.ToArray());

                var reportStream = new MemoryStream();
                finalDoc.Save(reportStream);
                finalDoc.Close(true);

                reportStream.Position = 0;

                string combinedReportPath = Path.Combine(reportsFolder, $"CombinedStudentBills_{DateTime.UtcNow:yyyyMMddHHmmss}.pdf");
                using (var fileStream = new FileStream(combinedReportPath, FileMode.Create, FileAccess.Write))
                {
                    await reportStream.CopyToAsync(fileStream);
                }
                _logger.LogInformation($"Saved combined student bills PDF: {combinedReportPath}");

                reportStream.Position = 0;
                string combinedUploadName = $"CombinedStudentBills_{DateTime.UtcNow:yyyyMMddHHmmss}";
                var combinedUploadedFileUrl = await fileUploadApi.UploadFileStream(reportStream, combinedUploadName, ".pdf");
                _logger.LogInformation($"Uploaded combined student bills PDF: {combinedUploadedFileUrl}");

                // Dispose all MemoryStreams
                foreach (var stream in streamArray)
                {
                    await stream.DisposeAsync();
                }
                await reportStream.DisposeAsync();

                var billReportUpdates = new
                {
                    studentBillReportList,
                    msgList[0].BillMetaData,
                    BatchDocumentUrl = combinedUploadedFileUrl,
                    msgList[0].TenantId,
                };
                
                //log the payload here
                _logger.LogInformation($"billReportUpdates_payload => {JsonConvert.SerializeObject(billReportUpdates)}");
                //produce to rabbit mq
               rabbitMqClient.Produce(topicExchangeType, studentBillReportUpdatesQueue, billReportUpdates);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error generating student bill report: {ex.Message}, {ex.StackTrace}");
                throw;
            }
        }
    }
    
    
}