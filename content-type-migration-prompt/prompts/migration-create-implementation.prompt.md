---
description: "Prompt that creates code to migrate data between two content types in Xperience by Kentico."
argument-hint: "Provide the necessary details about the source and target content types, using the template in /content-type-migration/input/migration-input-template.md.
tools:
  [
    "edit",
    "search",
    "runCommands",
    "kentico.docs.mcp/*",
    "problems",
    "fetch",
    "todos",
  ]

You are tasked with the process of creating a script to migrate data between two content types in Xperience by Kentico. The migration involves moving data from a source content type to a target content type, which may include updating references in other content types.

## Input Parameters

- **User input file:** `${input:userInputFile}` - The file that contains user input with specifications about the source and target content types, as well as any referencing content types. You must use these while creating the migration code.

## Steps to follow

- First, check all documentation links in the `./instructions/docs.instructions.md` file using Kentico Docs MCP
- Next, analyze the reference migration example in `./instructions/reference-migration-example.instructions.md`
- Then, create the migration code  in a new .cs file based on the specifications provided in the user input file. Make sure to follow the important rules listed below while implementing the migration code.
  - If there is a clear primary web project, add a `migrations` folder to it and add the migration code there. If not, ask the user to choose a project from the solution to add the migration code to.

## Important rules

- **Content item references** - When adding content item references to any field, avoid adding a new reference if the field already contains a reference with a matching `Identifier` GUID value.
- **Caching** - avoid caching, as the migration script will be a one-time use and caching could lead to stale data issues.
- **Publishing** - only work with published data, and publish any newly created or updated content items as part of the migration.
- **Parameterization** - Don't forget parameterization (like linked items) of retrieval when needed.
- **Add null checks** - Always validate that properties and required data are not null before accessing them.
- **Build frequently** - When done implementing the widget, build the project to check the current status. Fix and build until there are no errors related to the newly created migration code
- **Languages** - Migrate all language versions of the provided content types, not just the default language version.
- **No magic strings** - Avoid hardcoding strings directly in the code if possible. Use constants or `nameof` expressions.

If you are not completely confident about an API or feature behavior, use the Kentico Docs MCP server to check the Xperience by Kentico documentation on that topic.