# DocuTest
DocuTest implements a REST interface for some basic CRUD oppetaions for a Document persistance system. Developed to showcase the SOLID principles as well a performant way to query tens of millions of SQL records.

## Required Features
- ✓ CRUD operations for documents;
- ✓ Search for documents by metadata;
- ✓ Insert/Update/Delete files to from an existing document;
- ✓ Insert/Update/Delete metadata;

Also consider the following:
- ✓ Use a relational database;
- ✓ Support large volume of documents, e.g. 50M;
- ✓ Write some tests;
- ✓ Service calls should accept/return JSON obects;

## Optional Features
- ToDo: Suggest or implement a model for restricting document access based on the user, e.g. accountants can Insert/Update/Delete invoices, while others can only read them;
- ToDo: Use something simple to demonstrate the idea, e.g. hard code the list of users;

# Tables
## User
| Name | Type | Required | PK | FK |
| ------------- | ------------- | ------------- | ------------- | ------------- |
| Id | UNIQUEIDENTIFIER | ✓ | ✓ |  |
| Name | NVARCHAR(255) | ✓ |  |  |
| Email | NVARCHAR(255) | ✓ |  |  |

## Document
| Name | Type | Required | PK | FK |
| ------------- | ------------- | ------------- | ------------- | ------------- |
| Id | UNIQUEIDENTIFIER | ✓ | ✓ |  |
| Name | NVARCHAR(260) | ✓ |  |  |
| DocumentTypeId | UNIQUEIDENTIFIER | ✓ |  | DocumentType |
| UserId | UNIQUEIDENTIFIER | ✓ |  | User |

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
| Name | NVARCHAR(255) | ✓ |  |  |

## Metadata
| Name | Type | Required | PK | FK |
| ------------- | ------------- | ------------- | ------------- | ------------- |
| FileId | UNIQUEIDENTIFIER | ✓ | ✓ | File |
| Key | NVARCHAR(255) | ✓ | ✓ |  |
| Value | NVARCHAR(Max) | ✓ |  |  |

# Design Decisions
## Architecture and Structure
- A standard multi-layer architecture, utilizing an API layer, Application layer and DAL layer;
- A shared project, defining the models used for cross-layer communication;
- An SQL project, used to define the DB schema;
- A parallel data generator, used to populate 50 millions documents and up to 5 times as many files and metadata;

## Tools
- NewtonsoftJson used for serialization. System.Test.Json is still not mature enough and has trouble deserializing the `byte[]` type;
- Dapper used for its simplified interface, model binding, cancellation and parameter declaration capabilities;

## Data Access Mechanisms
- Every service acts as a UnitOfWork, taking care of opening and closing connections, starting transactions and logically encapsulating autonomous opperations;
- Every repository defines both the type and wording of the SQL query, does the data binding and adjacent table Join/Insert/Delete (when dealing with M:M or 1:M relationships for non-entity tables);

## Testing
- Tested the DocumentService class with both success and fail scenarios, making sure all the repository and transaction methods are called correctly;

## Considered, but not implemented:
- Model validation;
- Global error handling and logging;
- Further optimization and SQL indexes;
- Using specialized models for Get/Insert/Update opperations;
- Introducing seperate Request/Response/Exchange models to handle API layer declarative functionality like serialization or data annotations for validation;
- Introducing sorting and paging for the `get-by-metadata` route;