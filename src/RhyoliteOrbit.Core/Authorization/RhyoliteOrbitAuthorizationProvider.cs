using Abp.Authorization;
using Abp.Localization;
using Abp.MultiTenancy;

namespace RhyoliteOrbit.Authorization;

public class RhyoliteOrbitAuthorizationProvider : AuthorizationProvider
{
    public override void SetPermissions(IPermissionDefinitionContext context)
    {
        
         context.CreatePermission(PermissionNames.Dashboard, L("Dashboard"));
        
        context.CreatePermission(PermissionNames.Setups, L("Setups"));
        
        context.CreatePermission(PermissionNames.POS_ViewBranches, L("ViewBranches"));
        
        context.CreatePermission(PermissionNames.POS_CreateBranches, L("CreateBranches"));
        
        context.CreatePermission(PermissionNames.POS_UpdateBranches, L("UpdateBranches"));
        
        context.CreatePermission(PermissionNames.POS_DeleteBranches, L("DeleteBranches"));
        
        context.CreatePermission(PermissionNames.POS_ViewMerchants, L("ViewMerchants"));
        
        context.CreatePermission(PermissionNames.POS_CreateMerchant, L("CreateMerchant"));
        
        context.CreatePermission(PermissionNames.POS_UpdateMerchant, L("UpdateMerchant"));
        
        context.CreatePermission(PermissionNames.POS_DeleteMerchant, L("DeleteMerchant"));
        
        context.CreatePermission(PermissionNames.POS_ViewUnitsOfMeasurement, L("ViewUnitsOfMeasurement"));
        
        context.CreatePermission(PermissionNames.POS_CreateUnitsOfMeasurement, L("CreateUnitsOfMeasurement"));
        
        context.CreatePermission(PermissionNames.POS_UpdateUnitsOfMeasurement, L("UpdateUnitsOfMeasurement"));
        
        context.CreatePermission(PermissionNames.POS_DeleteUnitsOfMeasurement, L("DeleteUnitsOfMeasurement"));
        
        context.CreatePermission(PermissionNames.POS_ViewTaxesAndFees, L("ViewTaxesAndFees"));
        
        context.CreatePermission(PermissionNames.POS_CreateTaxesAndFees, L("CreateTaxesAndFees"));
        
        context.CreatePermission(PermissionNames.POS_UpdateTaxesAndFees, L("UpdateTaxesAndFees"));
        
        context.CreatePermission(PermissionNames.POS_DeleteTaxesAndFees, L("DeleteTaxesAndFees"));
        
        context.CreatePermission(PermissionNames.POS_ViewCoupons, L("ViewCoupons"));
        
        context.CreatePermission(PermissionNames.POS_CreateCoupons, L("CreateCoupons"));
        
        context.CreatePermission(PermissionNames.POS_UpdateCoupons, L("UpdateCoupons"));
        
        context.CreatePermission(PermissionNames.POS_DeleteCoupons, L("DeleteCoupons"));
        
        context.CreatePermission(PermissionNames.POS_ViewManufacturers, L("ViewManufacturers"));
        
        context.CreatePermission(PermissionNames.POS_CreateManufacturers, L("CreateManufacturer"));
        
        context.CreatePermission(PermissionNames.POS_UpdateManufacturers, L("UpdateManufacturer"));
        
        context.CreatePermission(PermissionNames.POS_DeleteManufacturers, L("DeleteManufacturer"));
        
        context.CreatePermission(PermissionNames.POS_ViewProductReviews, L("ViewProductReviews"));
        
        context.CreatePermission(PermissionNames.POS_DeleteProductReview, L("DeleteProductReview"));
        
        context.CreatePermission(PermissionNames.POS_ViewDeliveryChannels, L("ViewDeliveryChannels"));
        
        context.CreatePermission(PermissionNames.POS_CreateDeliveryChannel, L("CreateDeliveryChannel"));
        
        context.CreatePermission(PermissionNames.POS_UpdateDeliveryChannel, L("UpdateDeliveryChannel"));
        
        context.CreatePermission(PermissionNames.POS_DeleteDeliveryChannel, L("DeleteDeliveryChannel"));
        
        
        context.CreatePermission(PermissionNames.POS_ViewItemTags, L("ViewItemTags"));
        
        context.CreatePermission(PermissionNames.POS_CreateItemTag, L("CreateItemTag"));
        
        context.CreatePermission(PermissionNames.POS_UpdateItemTag, L("UpdateItemTag"));
        
        context.CreatePermission(PermissionNames.POS_DeleteItemTag, L("DeleteItemTag"));
        
        
        
        context.CreatePermission(PermissionNames.POS_ViewPickupPoints, L("ViewPickupPoints"));
        
        context.CreatePermission(PermissionNames.POS_CreatePickupPoints, L("CreatePickupPoint"));
        
        context.CreatePermission(PermissionNames.POS_UpdatePickupPoints, L("UpdatePickupPoint"));
        
        context.CreatePermission(PermissionNames.POS_DeletePickupPoints, L("DeletePickupPoint"));
        
        context.CreatePermission(PermissionNames.POS_ViewCustomerRanks, L("ViewCustomerRanks"));
        
        context.CreatePermission(PermissionNames.POS_CreateCustomerRanks, L("CreateCustomerRank"));
        
        context.CreatePermission(PermissionNames.POS_UpdateCustomerRanks, L("UpdateCustomerRank"));
        
        context.CreatePermission(PermissionNames.POS_DeleteCustomerRanks, L("DeleteCustomerRank"));
        
        context.CreatePermission(PermissionNames.POS_ViewCurrencyRates, L("ViewCurrencyRates"));
        
        context.CreatePermission(PermissionNames.POS_CreateCurrencyRates, L("CreateCurrencyRate"));
        
        context.CreatePermission(PermissionNames.POS_UpdateCurrencyRates, L("UpdateCurrencyRate"));
        
        context.CreatePermission(PermissionNames.POS_DeleteCurrencyRates, L("DeleteCurrencyRate"));
        
        context.CreatePermission(PermissionNames.POS_ViewSystemNumbers, L("ViewSystemNumbers"));
        
        context.CreatePermission(PermissionNames.POS_CreateSystemNumbers, L("CreateSystemNumbers"));
        
        context.CreatePermission(PermissionNames.POS_UpdateSystemNumbers, L("UpdateSystemNumbers"));
        
        context.CreatePermission(PermissionNames.POS_DeleteSystemNumbers, L("DeleteSystemNumbers"));
        
        context.CreatePermission(PermissionNames.POS_ViewCategories, L("ViewCategories"));
        
        context.CreatePermission(PermissionNames.POS_CreateCategories, L("CreateCategories"));
        
        context.CreatePermission(PermissionNames.POS_UpdateCategories, L("UpdateCategories"));
        
        context.CreatePermission(PermissionNames.POS_DeleteCategories, L("DeleteCategories"));
        
        context.CreatePermission(PermissionNames.POS_ViewProductsAndServices, L("ViewProductsAndServices"));
        
        context.CreatePermission(PermissionNames.POS_CreateProductsAndServices, L("CreateProductsAndServices"));
        
        context.CreatePermission(PermissionNames.POS_UpdateProductsAndServices, L("UpdateProductsAndServices"));
        
        context.CreatePermission(PermissionNames.POS_DeleteProductsAndServices, L("DeleteProductsAndServices"));
        
        context.CreatePermission(PermissionNames.POS_ViewStockTransfer, L("ViewStockTransfer"));
        
        context.CreatePermission(PermissionNames.POS_CreateStockTransfer, L("CreateStockTransfer"));
        
        context.CreatePermission(PermissionNames.POS_CancelStockTransfer, L("CancelStockTransfer"));
        
        context.CreatePermission(PermissionNames.POS_ViewStockReceipt, L("ViewStockReceipt"));
        
        context.CreatePermission(PermissionNames.POS_CreateStockReceipt, L("CreateStockReceipt"));
        
        context.CreatePermission(PermissionNames.POS_UpdateStockReceipt, L("UpdateStockReceipt"));
        
        context.CreatePermission(PermissionNames.POS_DeleteStockReceipt, L("DeleteStockReceipt"));
        
        context.CreatePermission(PermissionNames.POS_ManagePosSale, L("ManagePosSale"));
        
        context.CreatePermission(PermissionNames.POS_ViewSales, L("ViewSales"));
        
        context.CreatePermission(PermissionNames.POS_ViewShipments, L("ViewShipments"));
        
        context.CreatePermission(PermissionNames.POS_CreateShipments, L("CreateShipments"));
        
        context.CreatePermission(PermissionNames.POS_UpdateShipments, L("UpdateShipments"));
        
        context.CreatePermission(PermissionNames.POS_CancelShipments, L("CancelShipments"));
        
        context.CreatePermission(PermissionNames.POS_DeleteShipments, L("DeleteShipment"));
        
        context.CreatePermission(PermissionNames.POS_UpdateShipmentStatus, L("UpdateShipmentStatus"));
        
        
        // Document Management System
        var dmsRoot = context.CreatePermission(PermissionNames.Pages_Documents, L("Documents"));

        dmsRoot.CreateChildPermission(PermissionNames.DMS_ViewDocuments, L("DmsViewDocuments"));
        dmsRoot.CreateChildPermission(PermissionNames.DMS_CreateDocument, L("DmsCreateDocument"));
        dmsRoot.CreateChildPermission(PermissionNames.DMS_UpdateDocument, L("DmsUpdateDocument"));
        dmsRoot.CreateChildPermission(PermissionNames.DMS_DeleteDocument, L("DmsDeleteDocument"));
        dmsRoot.CreateChildPermission(PermissionNames.DMS_PurgeDocument, L("DmsPurgeDocument"));
        dmsRoot.CreateChildPermission(PermissionNames.DMS_DownloadDocument, L("DmsDownloadDocument"));
        dmsRoot.CreateChildPermission(PermissionNames.DMS_PreviewDocument, L("DmsPreviewDocument"));
        dmsRoot.CreateChildPermission(PermissionNames.DMS_PrintDocument, L("DmsPrintDocument"));
        dmsRoot.CreateChildPermission(PermissionNames.DMS_CheckOutDocument, L("DmsCheckOutDocument"));
        dmsRoot.CreateChildPermission(PermissionNames.DMS_ForceUnlockDocument, L("DmsForceUnlockDocument"));
        dmsRoot.CreateChildPermission(PermissionNames.DMS_ManageVersions, L("DmsManageVersions"));
        dmsRoot.CreateChildPermission(PermissionNames.DMS_ManageFolders, L("DmsManageFolders"));
        dmsRoot.CreateChildPermission(PermissionNames.DMS_ManageCollections, L("DmsManageCollections"));
        dmsRoot.CreateChildPermission(PermissionNames.DMS_ManageTags, L("DmsManageTags"));
        dmsRoot.CreateChildPermission(PermissionNames.DMS_ManageTemplates, L("DmsManageTemplates"));
        dmsRoot.CreateChildPermission(PermissionNames.DMS_ManageIdentifiers, L("DmsManageIdentifiers"));
        dmsRoot.CreateChildPermission(PermissionNames.DMS_ManagePermissions, L("DmsManagePermissions"));
        dmsRoot.CreateChildPermission(PermissionNames.DMS_ShareDocument, L("DmsShareDocument"));
        dmsRoot.CreateChildPermission(PermissionNames.DMS_ManageShares, L("DmsManageShares"));
        dmsRoot.CreateChildPermission(PermissionNames.DMS_ManageWorkflows, L("DmsManageWorkflows"));
        dmsRoot.CreateChildPermission(PermissionNames.DMS_StartWorkflow, L("DmsStartWorkflow"));
        dmsRoot.CreateChildPermission(PermissionNames.DMS_ParticipateInWorkflow, L("DmsParticipateInWorkflow"));
        dmsRoot.CreateChildPermission(PermissionNames.DMS_ViewAuditTrail, L("DmsViewAuditTrail"));
        dmsRoot.CreateChildPermission(PermissionNames.DMS_ExportAuditTrail, L("DmsExportAuditTrail"));
        dmsRoot.CreateChildPermission(PermissionNames.DMS_ManageRetention, L("DmsManageRetention"));
        dmsRoot.CreateChildPermission(PermissionNames.DMS_ApplyLegalHold, L("DmsApplyLegalHold"));
        dmsRoot.CreateChildPermission(PermissionNames.DMS_ManageSignatures, L("DmsManageSignatures"));
        dmsRoot.CreateChildPermission(PermissionNames.DMS_SignDocument, L("DmsSignDocument"));
        dmsRoot.CreateChildPermission(PermissionNames.DMS_ManageAnnotations, L("DmsManageAnnotations"));
        dmsRoot.CreateChildPermission(PermissionNames.DMS_BulkImport, L("DmsBulkImport"));
        dmsRoot.CreateChildPermission(PermissionNames.DMS_BulkExport, L("DmsBulkExport"));
        dmsRoot.CreateChildPermission(PermissionNames.DMS_ManageSettings, L("DmsManageSettings"));
        dmsRoot.CreateChildPermission(PermissionNames.DMS_ManageQuotas, L("DmsManageQuotas"));
        dmsRoot.CreateChildPermission(PermissionNames.DMS_UseAiFeatures, L("DmsUseAiFeatures"));

        context.CreatePermission(PermissionNames.Pages_Users, L("Users"));
        context.CreatePermission(PermissionNames.Pages_Users_Activation, L("UsersActivation"));
        context.CreatePermission(PermissionNames.Pages_Roles, L("Roles"));
        context.CreatePermission(PermissionNames.Pages_Tenants, L("Tenants"), multiTenancySides: MultiTenancySides.Host);
    }

    private static ILocalizableString L(string name)
    {
        return new LocalizableString(name, RhyoliteOrbitConsts.LocalizationSourceName);
    }
}
