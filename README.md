# DocuTest
DocuTest implements a REST interface for some basic CRUD oppetaions for a Document persistance system. Developed to showcase the SOLID principles as well a performant way to query millions of SQL records.

## Required Features
- CRUD operations for documents;
- Search for documents by metadata;
- Add/Remove/Update files to eixisting document;
- Add/Rremove/Update metadata;


Also consider the following:
- Use a relational database;
- Support large volume of documents, e.g. 50M;
- Write some tests;
- Service calls should accept/return json obects;

## Optional Features
- Suggest or Implement a model for restricting documents access based on the user, e.g. accountants can Store/Update/Delete invoices, others can only read them;
- Use something simple to demonstrate the idea,  e.g. hard code the list of users;

# Tables
## User
| Name | Type | Required | PK | FK |
| ------------- | ------------- | ------------- | ------------- | ------------- |
| Id | UNIQUEIDENTIFIER | ✓ | ✓ |  |
| Name | NVARCHAR(100) | ✓ |  |  |
| Email | NVARCHAR(255) | ✓ |  |  |

## Document
| Name | Type | Required | PK | FK |
| ------------- | ------------- | ------------- | ------------- | ------------- |
| Id | UNIQUEIDENTIFIER | ✓ | ✓ |  |
| Name | NVARCHAR(260) | ✓ |  |  |
| DocumentTypeId | UNIQUEIDENTIFIER | ✓ |  | DocumentType |
| DocumentTypeId | UNIQUEIDENTIFIER | ✓ |  | User |

## File
| Name | Type | Required | PK | FK |
| ------------- | ------------- | ------------- | ------------- | ------------- |
| Id | UNIQUEIDENTIFIER | ✓ | ✓ |  |
| Name | NVARCHAR(260) | ✓ |  |  |
| Extension | NVARCHAR(260) | ✓ |  |  |
| Content | VARBINARY(MAX) | ✓ |  |  |

## DocumentFile
| Name | Type | Required | PK | FK |
| ------------- | ------------- | ------------- | ------------- | ------------- |
| DocumentId | UNIQUEIDENTIFIER | ✓ | ✓ | Document |
| FileId | UNIQUEIDENTIFIER | ✓ | ✓ | File |

## DocumentType
| Name | Type | Required | PK | FK |
| ------------- | ------------- | ------------- | ------------- | ------------- |
| Id | UNIQUEIDENTIFIER | ✓ | ✓ |  |
| Name | NVARCHAR(260) | ✓ |  |  |

## Metadata
| Name | Type | Required | PK | FK |
| ------------- | ------------- | ------------- | ------------- | ------------- |
| Id | UNIQUEIDENTIFIER | ✓ | ✓ |  |
| FileId | UNIQUEIDENTIFIER | ✓ |  | File |
| Key | NVARCHAR(255) | ✓ |  |  |
| Value | NVARCHAR(Max) | ✓ |  |  |
