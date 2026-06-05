# Content type migration request

<!-- 
  INSTRUCTIONS FOR THE HUMAN:
  Fill in each section below with the details of your specific migration.
  Then provide this file (along with the reference example and docs instructions)
  to your AI coding agent to generate the migration code.

  - Replace placeholder text in {CURLY BRACES} with your actual values.
  - Paste full generated C# classes and XML form definitions where indicated.
  - Delete the "Referencing content types" section entirely if your migration
    does not need to update references in other content types.
  - In the field mapping table, add or remove rows as needed. Use the
    "Transformation" column to describe how to handle each field.
-->

## What to migrate

- **Source content type**: `{Namespace.SourceClassName}` ({Source display name}) — {brief description, e.g., "the old reusable content type being replaced"}
- **Target content type**: `{Namespace.TargetClassName}` ({Target display name}) — {brief description, e.g., "the new reusable content type; uses XyzSchema reusable field schema"}
- **Referencing content type(s)** (if any): `{Namespace.ReferencingClassName}` ({Referencing display name}) — {brief description, e.g., "page wrapper that holds a reference field pointing to the source type; update to point to the target type"}

## Content type details

### Source content type <!-- duplicate this whole section if you need to pull from multiple source content types, updating headings with numbers, e.g., `## Source content type 1` -->

- {Source display name}

#### C# class

- `path within your repository to the generated class for the content type`

#### ClassFormDefinition

```xml
<!-- Paste the ClassFormDefinition XML for the source content type here. (Use this query: SELECT ClassFormDefinition FROM CMS_Class WHERE ClassName = '{SourceClassName}') -->
```

### Target content type

{Target display name}

#### C# class

- `path within your repository to the generated class for the content type`
- `path to relevant reusable field schema interface`


#### ClassFormDefinition

```xml
<!-- Paste the ClassFormDefinition XML for the target content type here. (Use this query: SELECT ClassFormDefinition FROM CMS_Class WHERE ClassName = '{TargetClassName}') -->
<!-- If the target uses a reusable field schema, also paste the ClassFormDefinition from the ContentItemCommonData class entry, which contains the schema definition. (Use this query: SELECT ClassFormDefinition FROM CMS_Class WHERE ClassName = 'cms.contentitemcommondata')-->
```

### Referencing content types

<!-- Delete this entire section if no other content types reference the source type. -->
<!-- Duplicate the sub-sections below if multiple content types reference the source. -->

{Referencing display name}

{Describe how this content type references the source type and what the migration
should do. For example: "This page content type has a `PageContent` field that 
references the old source type. The migration should update it to reference the 
new target type via the `PageNewContent` field, and clear the old field."}

#### C# class

`path within your repository to the generated class for the content type`

#### ClassFormDefinition

```xml
<!-- Paste the ClassFormDefinition XML for the referencing content type here. -->
```

## Field mapping

<!-- 
  One row per field. Include ALL fields from both source and target.
  - For source fields with no target equivalent, set Target Field to "(drop)".
  - For target fields with no source equivalent, set Source Field to "(none)".
  - Use the Transformation column to describe any conversion logic.
-->

| Source Field | Target Field | Type | Transformation |
|---|---|---|---|
| `{SourceField1}` | `{TargetField1}` | `{type, e.g., text(300)}` | {e.g., Direct copy} |
| `{SourceField2}` | `{TargetField2}` | `{type}` | {e.g., Direct copy; wrap in ContentItemReference list} |
| `{SourceRefField}` | `{TargetRefField}` | `contentitemreference` | {e.g., Re-map: look up new target GUID by old source codename} |
| *(none)* | `{NewTargetField}` | `{type}` | {e.g., Not migrated; leave empty / set default value} |
| `{DroppedSourceField}` | *(drop)* | `{type}` | {e.g., Not carried over to new type} |

## Additional context

<!-- Optional. Add any extra details the agent should know, such as: -->
<!-- - Special transformation logic for specific fields -->
<!-- - Whether content items exist in multiple languages -->
<!-- - Whether scheduled publish/unpublish dates should be preserved -->
<!-- - Any fields that require data from external sources or lookups -->
<!-- - Business rules or constraints on the migration -->

{Add any additional notes here, or delete this section if not needed.}
