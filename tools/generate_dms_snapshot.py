#!/usr/bin/env python3
"""Generates the EF Core model snapshot additions for the DMS module.

Reads RhyoliteOrbitDbContextModelSnapshot.cs, inserts the new properties into the
existing DMS entity blocks (alphabetical order, Id first) and appends blocks for the
new entities. Also emits the Designer.cs for the hand written migration, which must
contain the full target model.
"""
import re
import sys

SNAPSHOT = 'src/RhyoliteOrbit.EntityFrameworkCore/Migrations/RhyoliteOrbitDbContextModelSnapshot.cs'
DESIGNER_OUT = 'src/RhyoliteOrbit.EntityFrameworkCore/Migrations/20261012120000_DmsModuleV1.Designer.cs'
MIGRATION_ID = '20261012120000_DmsModuleV1'

NS = 'RhyoliteOrbit.Models.Dms'

# property helpers -----------------------------------------------------------
def p_str(name, length=None, col=None, computed=None):
    out = f'b.Property<string>("{name}")\n'
    if length:
        out += f'    .HasMaxLength({length})\n'
    out += f'    .HasColumnType("{col or ("character varying(%d)" % length)}")\n'
    if computed:
        out += f'    .HasComputedColumnSql("{computed}", true)\n'
    elif computed is None and False:
        pass
    return out

def p(name, clr, col, computed=None):
    out = f'b.Property<{clr}>("{name}")\n    .HasColumnType("{col}")\n'
    if computed:
        out += f'    .HasComputedColumnSql("{computed}", true)\n'
    return out

VEC = "to_tsvector('simple', coalesce(\\\"Name\\\", '') || ' ' || coalesce(\\\"Title\\\", '') || ' ' || coalesce(\\\"Description\\\", '') || ' ' || coalesce(\\\"CustomId\\\", ''))"
VEC_V = "to_tsvector('simple', coalesce(\\\"ExtractedText\\\", ''))"

# existing entities: new properties only ------------------------------------
EXTEND = {
    'DmsDocument': [
        p('CollectionId', 'Guid', 'uuid'),
        p('CollectionId2', 'Guid', 'uuid'),  # placeholder removed below
    ],
}

def build_document_props():
    return [
        p('CollectionId', 'Guid', 'uuid'),
        p_str('CustomId', 100),
        p_str('Extension', 30),
        p_str('Language', 20),
        p_str('PreviewPath', 500),
        p_str('StorageKey', 500),
        p_str('StorageProvider', 30),
        p_str('ThumbnailPath', 500),
        p('AiStatus', 'int', 'integer'),
        p('CheckedOutAt', 'DateTime?', 'timestamp without time zone'),
        p('CheckedOutBy', 'long?', 'bigint'),
        p('CurrentVersionId', 'Guid?', 'uuid'),
        p('ExpiryDate', 'DateTime?', 'timestamp without time zone'),
        p('FileSize', 'long', 'bigint'),
        p('IndexStatus', 'int', 'integer'),
        p('IndexedAt', 'DateTime?', 'timestamp without time zone'),
        p('IsCheckedOut', 'bool', 'boolean'),
        p('IsOnLegalHold', 'bool', 'boolean'),
        p('IsWorm', 'bool', 'boolean'),
        p('LockExpiry', 'DateTime?', 'timestamp without time zone'),
        p('OcrStatus', 'int', 'integer'),
        p('OwnerId', 'long', 'bigint'),
        p('PageCount', 'int', 'integer'),
        p('RetentionEventDate', 'DateTime?', 'timestamp without time zone'),
        p('RetentionPolicyId', 'Guid?', 'uuid'),
        p('ScanStatus', 'int', 'integer'),
        p('SearchVector', 'string', 'tsvector', VEC),
    ]

def build_folder_props():
    return [
        p('CollectionId', 'Guid', 'uuid'),
        p('Depth', 'int', 'integer'),
        p('IdentifierId', 'Guid?', 'uuid'),
        p('InheritPermissions', 'bool', 'boolean'),
        p('IsSystem', 'bool', 'boolean'),
        p('OwnerId', 'long', 'bigint'),
        p('ParentId', 'Guid?', 'uuid'),
        p('TemplateId', 'Guid?', 'uuid'),
    ]

def build_collection_props():
    return [
        p('IsDefault', 'bool', 'boolean'),
        p('StorageQuotaBytes', 'long', 'bigint'),
    ]

def build_tag_props():
    return [p('IsSystem', 'bool', 'boolean')]

def build_template_props():
    return [
        p('RequiresApproval', 'bool', 'boolean'),
        p('RetentionPolicyId', 'Guid?', 'uuid'),
        p('VersionScheme', 'int', 'integer'),
        p('WorkflowTemplateId', 'Guid?', 'uuid'),
        p_str('AllowedFileTypes', None, 'jsonb'),
    ]

def build_attribute_props():
    return [
        p('IsSearchable', 'bool', 'boolean'),
        p('IsSensitive', 'bool', 'boolean'),
        p_str('ValidationRegex', 200),
    ]

def build_identifier_props():
    return [
        p('IsActive', 'bool', 'boolean'),
        p('LastGeneratedAt', 'DateTime?', 'timestamp without time zone'),
        p('ResetPolicy', 'int', 'integer'),
    ]

def build_comment_props():
    return [
        p('ResolvedAt', 'DateTime?', 'timestamp without time zone'),
        p('ResolvedById', 'long?', 'bigint'),
        p('VersionId', 'Guid?', 'uuid'),
        p_str('AnchorJson', None, 'jsonb'),
    ]

def build_wf_template_props():
    return [
        p('IsActive', 'bool', 'boolean'),
        p('Version', 'int', 'integer'),
    ]

def build_wf_instance_props():
    return [
        p('DueDate', 'DateTime?', 'timestamp without time zone'),
        p('LastActionAt', 'DateTime?', 'timestamp without time zone'),
        p('StartedById', 'long', 'bigint'),
        p_str('TemplateName', 100),
        p_str('TemplateSnapshotJson', None, 'jsonb'),
        p_str('VariablesJson', None, 'jsonb'),
    ]

def build_wf_task_props():
    return [
        p('DelegatedFrom', 'long?', 'bigint'),
        p('DueDate', 'DateTime?', 'timestamp without time zone'),
        p('SignatureId', 'Guid?', 'uuid'),
        p('StepIndex', 'int', 'integer'),
        p_str('OutcomeJson', None, 'jsonb'),
        p_str('StepName', 100),
    ]

EXTENDERS = {
    'DmsDocument': build_document_props,
    'DmsFolder': build_folder_props,
    'DmsCollection': build_collection_props,
    'DmsTag': build_tag_props,
    'DmsDocumentTemplate': build_template_props,
    'DmsCustomAttributes': build_attribute_props,
    'DmsCustomIdentifier': build_identifier_props,
    'DmsComment': build_comment_props,
    'DmsWorkflowTemplate': build_wf_template_props,
    'DmsWorkflowInstance': build_wf_instance_props,
    'DmsWorkflowTask': build_wf_task_props,
}

# new entities ---------------------------------------------------------------
AUDIT_PROPS = [
    'b.Property<Guid>("Id")\n    .ValueGeneratedOnAdd()\n    .HasColumnType("uuid")',
    'b.Property<DateTime>("CreationTime")\n    .HasColumnType("timestamp without time zone")',
    'b.Property<long?>("CreatorUserId")\n    .HasColumnType("bigint")',
    'b.Property<long?>("DeleterUserId")\n    .HasColumnType("bigint")',
    'b.Property<DateTime?>("DeletionTime")\n    .HasColumnType("timestamp without time zone")',
    'b.Property<bool>("IsDeleted")\n    .HasColumnType("boolean")',
    'b.Property<DateTime?>("LastModificationTime")\n    .HasColumnType("timestamp without time zone")',
    'b.Property<long?>("LastModifierUserId")\n    .HasColumnType("bigint")',
]

CREATION_PROPS = [
    'b.Property<Guid>("Id")\n    .ValueGeneratedOnAdd()\n    .HasColumnType("uuid")',
    'b.Property<DateTime>("CreationTime")\n    .HasColumnType("timestamp without time zone")',
    'b.Property<long?>("CreatorUserId")\n    .HasColumnType("bigint")',
]


def render_entity(cls, table, props):
    """Render a complete entity block in EF snapshot formatting."""
    ordered = sorted(props, key=lambda e: prop_name(e) or '')
    entries = ordered + ['b.HasKey("Id")']
    body = '\n\n'.join(indent_entry(e) for e in entries)
    return ('modelBuilder.Entity("%s.%s", b =>\n'
            '                {\n'
            '%s\n\n'
            '                    b.ToTable("%s");\n'
            '                });') % (NS, cls, body, table)


def full_audited(cls, table, props):
    return render_entity(cls, table, AUDIT_PROPS + list(props))


def creation_audited(cls, table, props):
    return render_entity(cls, table, CREATION_PROPS + list(props))


NEW_ENTITIES = [
    ('DmsAnnotation', 'DmsAnnotations', full_audited, [
        p('Color', 'string', 'character varying(20)'),
        p('CreatedById', 'long', 'bigint'),
        p('DocumentId', 'Guid', 'uuid'),
        p('IsRedaction', 'bool', 'boolean'),
        p('PageNumber', 'int', 'integer'),
        p('TenantId', 'int', 'integer'),
        p('Type', 'int', 'integer'),
        p('VersionId', 'Guid', 'uuid'),
        p_str('GeometryJson', None, 'jsonb'),
        p_str('Text', 1000),
    ]),
    ('DmsDocumentVersion', 'DmsDocumentVersions', full_audited, [
        p('Checksum', 'string', 'character varying(64)'),
        p('DocumentId', 'Guid', 'uuid'),
        p('Extension', 'string', 'character varying(30)'),
        p('FileSize', 'long', 'bigint'),
        p('IsCurrent', 'bool', 'boolean'),
        p('MinorNumber', 'int', 'integer'),
        p('OcrApplied', 'bool', 'boolean'),
        p('PageCount', 'int', 'integer'),
        p('TenantId', 'int', 'integer'),
        p('VersionNumber', 'int', 'integer'),
        p_str('ChangeNote', 500),
        p_str('ExtractedText', None, 'text'),
        p_str('FileName', 255),
        p_str('Language', 20),
        p_str('MimeType', 255),
        p_str('PreviewPath', 500),
        p_str('StorageKey', 500),
        p_str('StorageProvider', 30),
        p_str('ThumbnailPath', 500),
        p_str('VersionLabel', 50),
        p('SearchVector', 'string', 'tsvector', VEC_V),
    ]),
    ('DmsAclEntry', 'DmsAclEntries', full_audited, [
        p('Effect', 'int', 'integer'),
        p('IsInherited', 'bool', 'boolean'),
        p('Permission', 'int', 'integer'),
        p('SecurableId', 'Guid', 'uuid'),
        p('SecurableType', 'int', 'integer'),
        p('SubjectId', 'long', 'bigint'),
        p('SubjectType', 'int', 'integer'),
        p('TenantId', 'int', 'integer'),
    ]),
    ('DmsAuditEntry', 'DmsAuditEntries', creation_audited, [
        p('Action', 'int', 'integer'),
        p('ActorId', 'long?', 'bigint'),
        p('DocumentId', 'Guid?', 'uuid'),
        p('FolderId', 'Guid?', 'uuid'),
        p('Sequence', 'long', 'bigint'),
        p('TenantId', 'int', 'integer'),
        p_str('DetailsJson', None, 'jsonb'),
        p_str('EntryHash', 64),
        p_str('IpAddress', 100),
        p_str('PreviousHash', 64),
        p_str('UserAgent', 300),
    ]),
    ('DmsShare', 'DmsShares', full_audited, [
        p('AllowDownload', 'bool', 'boolean'),
        p('AllowPreview', 'bool', 'boolean'),
        p('AllowUpload', 'bool', 'boolean'),
        p('CreatedById', 'long', 'bigint'),
        p('DocumentId', 'Guid?', 'uuid'),
        p('DownloadCount', 'int', 'integer'),
        p('ExpiresAt', 'DateTime?', 'timestamp without time zone'),
        p('FolderId', 'Guid?', 'uuid'),
        p('IsRevoked', 'bool', 'boolean'),
        p('LastAccessedAt', 'DateTime?', 'timestamp without time zone'),
        p('MaxDownloads', 'int?', 'integer'),
        p('RequiresWatermark', 'bool', 'boolean'),
        p('ShareType', 'int', 'integer'),
        p('SharedWithUserId', 'long?', 'bigint'),
        p('TenantId', 'int', 'integer'),
        p_str('PasswordHash', 200),
        p_str('SharedWithEmail', 200),
        p_str('Token', 128),
    ]),
    ('DmsSavedSearch', 'DmsSavedSearches', full_audited, [
        p('IsGlobal', 'bool', 'boolean'),
        p('LastRunAt', 'DateTime?', 'timestamp without time zone'),
        p('TenantId', 'int', 'integer'),
        p('UserId', 'long', 'bigint'),
        p_str('CriteriaJson', None, 'jsonb'),
        p_str('Name', 100),
    ]),
    ('DmsRetentionPolicy', 'DmsRetentionPolicies', full_audited, [
        p('AllowLegalHold', 'bool', 'boolean'),
        p('DispositionAction', 'int', 'integer'),
        p('IsActive', 'bool', 'boolean'),
        p('RetentionDays', 'int', 'integer'),
        p('TenantId', 'int', 'integer'),
        p('Trigger', 'int', 'integer'),
        p_str('Description', 250),
        p_str('FolderIds', None, 'jsonb'),
        p_str('Name', 100),
    ]),
    ('DmsLegalHold', 'DmsLegalHolds', full_audited, [
        p('AppliedById', 'long', 'bigint'),
        p('IsActive', 'bool', 'boolean'),
        p('ReleasedAt', 'DateTime?', 'timestamp without time zone'),
        p('TenantId', 'int', 'integer'),
        p_str('DocumentIds', None, 'jsonb'),
        p_str('FolderIds', None, 'jsonb'),
        p_str('Name', 100),
        p_str('Reason', 250),
    ]),
    ('DmsSignature', 'DmsSignatures', full_audited, [
        p('DocumentId', 'Guid', 'uuid'),
        p('SignedAt', 'DateTime', 'timestamp without time zone'),
        p('SignatureType', 'int', 'integer'),
        p('SignerId', 'long', 'bigint'),
        p('TenantId', 'int', 'integer'),
        p('VersionId', 'Guid', 'uuid'),
        p_str('CertificateThumbprint', 100),
        p_str('IpAddress', 50),
        p_str('Reason', 200),
        p_str('SignatureImagePath', 500),
        p_str('SignedHash', 64),
    ]),
    ('DmsEntityLink', 'DmsEntityLinks', full_audited, [
        p('CreatedById', 'long', 'bigint'),
        p('DocumentId', 'Guid', 'uuid'),
        p('EntityType', 'int', 'integer'),
        p('LinkType', 'int', 'integer'),
        p('TenantId', 'int', 'integer'),
        p_str('EntityId', 100),
        p_str('Note', 250),
    ]),
    ('DmsEmbedding', 'DmsEmbeddings', creation_audited, [
        p('ChunkIndex', 'int', 'integer'),
        p('DocumentId', 'Guid', 'uuid'),
        p('TenantId', 'int', 'integer'),
        p('VersionId', 'Guid', 'uuid'),
        p_str('ChunkText', None, 'text'),
        p_str('Model', 50),
        p_str('VectorJson', None, 'jsonb'),
    ]),
    ('DmsDocumentEvent', 'DmsDocumentEvents', creation_audited, [
        p('AggregateId', 'Guid', 'uuid'),
        p('OccurredAt', 'DateTime', 'timestamp without time zone'),
        p('ProcessedAt', 'DateTime?', 'timestamp without time zone'),
        p('RetryCount', 'int', 'integer'),
        p('TenantId', 'int', 'integer'),
        p_str('EventType', 100),
        p_str('LastError', 500),
        p_str('PayloadJson', None, 'jsonb'),
    ]),
    ('DmsQuotaUsage', 'DmsQuotaUsages', full_audited, [
        p('BytesUsed', 'long', 'bigint'),
        p('ComputedAt', 'DateTime', 'timestamp without time zone'),
        p('DocumentCount', 'int', 'integer'),
        p('TenantId', 'int', 'integer'),
        p('VersionCount', 'int', 'integer'),
    ]),
    ('DmsIngestJob', 'DmsIngestJobs', full_audited, [
        p('CompletedAt', 'DateTime?', 'timestamp without time zone'),
        p('CreatedById', 'long', 'bigint'),
        p('FailedItems', 'int', 'integer'),
        p('JobType', 'int', 'integer'),
        p('ProcessedItems', 'int', 'integer'),
        p('StartedAt', 'DateTime?', 'timestamp without time zone'),
        p('Status', 'int', 'integer'),
        p('TenantId', 'int', 'integer'),
        p('TotalItems', 'int', 'integer'),
        p_str('ErrorLog', None, 'text'),
        p_str('ResultPath', None, 'text'),
        p_str('SourcePath', None, 'text'),
    ]),
    ('DmsSettings', 'DmsSettings', full_audited, [
        p('AiEnabled', 'bool', 'boolean'),
        p('AntivirusEnabled', 'bool', 'boolean'),
        p('EnforceVersioning', 'bool', 'boolean'),
        p('MaxFileSizeMb', 'int', 'integer'),
        p('OcrEnabled', 'bool', 'boolean'),
        p('QuotaGb', 'int', 'integer'),
        p('TenantId', 'int', 'integer'),
        p('TrashRetentionDays', 'int', 'integer'),
        p_str('AllowedFileTypes', None, 'jsonb'),
        p_str('DefaultLanguage', 30),
        p_str('LocalRootPath', 300),
        p_str('OcrLanguages', None, 'jsonb'),
        p_str('StorageProvider', 30),
        p_str('WatermarkText', 200),
    ]),
]


def prop_name(entry):
    m = re.match(r'b\.Property<[^>]+>\("([^"]+)"\)', entry)
    return m.group(1) if m else None


def split_entries(block):
    """Split the property region of an entity block into individual entries."""
    lines = block.split('\n')
    entries = []
    current = None
    for line in lines:
        stripped = line.strip()
        if stripped.startswith('b.Property<'):
            if current is not None:
                entries.append(current)
            current = [stripped]
        elif current is not None:
            if stripped == '':
                continue
            current.append(stripped)
            if stripped.endswith(';'):
                entries.append(current)
                current = None
    if current is not None:
        entries.append(current)
    return entries


def indent_entry(text):
    """Re-indent a property entry to the EF snapshot style (20/24 spaces)."""
    lines = [l for l in text.split('\n') if l.strip()]
    out = ['                    ' + lines[0].strip()]
    for line in lines[1:]:
        out.append('                        ' + line.strip())
    joined = '\n'.join(out)
    return joined if joined.endswith(';') else joined + ';'


def insert_props(block, new_entries):
    """Merge new property entries into an existing entity block: entries with a
    known name replace the old definition, unknown names are inserted. Ordering
    follows the EF convention (Id first, then alphabetical)."""
    lines = block.split('\n')
    has_key_idx = next(i for i, l in enumerate(lines) if 'b.HasKey(' in l)
    header = lines[:has_key_idx]
    header_idx = next(i for i, l in enumerate(header) if l.strip().startswith('b.Property<'))

    merged = {}
    for entry in split_entries('\n'.join(header[header_idx:])):
        merged[prop_name('\n'.join(entry))] = '\n'.join(entry)

    for entry in new_entries:
        text = entry.rstrip('\n').rstrip()
        if not text.endswith(';'):
            text += ';'
        merged[prop_name(text)] = text

    ordered = sorted(merged.items(), key=lambda kv: (0, '') if kv[0] == 'Id' else (1, kv[0] or ''))
    body = '\n\n'.join(indent_entry(text) for _, text in ordered)
    rebuilt = header[:header_idx] + [body, ''] + lines[has_key_idx:]
    return '\n'.join(rebuilt)


def main():
    raw = open(SNAPSHOT, 'rb').read()
    bom = raw.startswith(b'\xef\xbb\xbf')
    src = raw.decode('utf-8-sig')

    # 1) extend existing entities
    for type_name, builder in EXTENDERS.items():
        marker = 'modelBuilder.Entity("%s.%s", b =>' % (NS, type_name)
        idx = src.find(marker)
        if idx < 0:
            print('WARN: entity block not found:', type_name)
            continue
        end = src.find('b.ToTable(', idx)
        end = src.find('});', end) + len('});\n')
        block = src[idx:end]
        updated = insert_props(block, builder())
        src = src[:idx] + updated + src[end:]

    # 2) append new entities before the trailing pragma restore
    anchor = '#pragma warning restore 612, 618'
    idx = src.find(anchor)
    if idx < 0:
        print('ERROR: anchor not found')
        sys.exit(1)
    new_blocks = '\n            '.join(fn(cls, tbl, props) for cls, tbl, fn, props in NEW_ENTITIES)
    src = src[:idx] + '            ' + new_blocks + '\n' + src[idx:]

    open(SNAPSHOT, 'wb').write((b'\xef\xbb\xbf' if bom else b'') + src.encode('utf-8'))
    print('snapshot updated')

    # 3) designer
    model_body = src[src.find('protected override void BuildModel(ModelBuilder modelBuilder)') + len('protected override void BuildModel(ModelBuilder modelBuilder)'):]
    model_body = model_body.strip()
    # strip the surrounding braces of BuildModel
    assert model_body.startswith('{') and model_body.rstrip().endswith('}')
    inner = model_body[1:model_body.rstrip().rfind('}')].strip()
    # pragmas wrap the whole BuildModel method and the trailing braces belong to
    # the snapshot's own structure: keep only the statements.
    if inner.startswith('#pragma warning disable 612, 618'):
        inner = inner[len('#pragma warning disable 612, 618'):]
    cut = inner.rfind('#pragma warning restore 612, 618')
    if cut >= 0:
        inner = inner[:cut]
    inner = inner.strip()

    designer = '''// <auto-generated />
using System;
using System.Collections.Generic;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using RhyoliteOrbit.EntityFrameworkCore;
using RhyoliteOrbit.Models.Dms;
using RhyoliteOrbit.Models.GeneralLedger;
using RhyoliteOrbit.Models.Payroll;
using RhyoliteOrbit.Models.Pos;
using RhyoliteOrbit.Models.ProductionPlanning;
using RhyoliteOrbit.Models.SchoolManager;
using RhyoliteOrbit.Models.Shared;
using RhyoliteOrbit.Models.Wssd;

#nullable disable

namespace RhyoliteOrbit.Migrations
{
    [DbContext(typeof(RhyoliteOrbitDbContext))]
    [Migration("%s")]
    partial class DmsModuleV1
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
%s
        }
    }
}
''' % (MIGRATION_ID, inner)

    open(DESIGNER_OUT, 'w', encoding='utf-8').write(designer)
    print('designer written')


if __name__ == '__main__':
    main()
