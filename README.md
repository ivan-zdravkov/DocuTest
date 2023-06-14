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
- ✓ Suggest or implement a model for restricting document access based on the user, e.g. accountants can Insert/Update/Delete invoices, while others can only read them;
- ✓ Use something simple to demonstrate the idea, e.g. hard code the list of users;

# Tables
## [User](./DocuTest.Data.Main.Generator/User.sql)
| Name | Type | Required | PK | FK |
| ------------- | ------------- | ------------- | ------------- | ------------- |
| Id | UNIQUEIDENTIFIER | ✓ | ✓ |  |
| Name | NVARCHAR(255) | ✓ |  |  |
| Email | NVARCHAR(255) | ✓ |  |  |

## [Document](./DocuTest.Data.Main.Generator/Document.sql)
| Name | Type | Required | PK | FK |
| ------------- | ------------- | ------------- | ------------- | ------------- |
| Id | UNIQUEIDENTIFIER | ✓ | ✓ |  |
| Name | NVARCHAR(260) | ✓ |  |  |
| DocumentTypeId | UNIQUEIDENTIFIER | ✓ |  | DocumentType |
| UserId | UNIQUEIDENTIFIER | ✓ |  | User |

## [File](./DocuTest.Data.Main.Generator/File.sql)
| Name | Type | Required | PK | FK |
| ------------- | ------------- | ------------- | ------------- | ------------- |
| Id | UNIQUEIDENTIFIER | ✓ | ✓ |  |
| Name | NVARCHAR(260) | ✓ |  |  |
| Extension | NVARCHAR(260) | ✓ |  |  |
| Content | VARBINARY(MAX) | ✓ |  |  |

## [DocumentFile](./DocuTest.Data.Main.Generator/DocumentFile.sql)
| Name | Type | Required | PK | FK |
| ------------- | ------------- | ------------- | ------------- | ------------- |
| DocumentId | UNIQUEIDENTIFIER | ✓ | ✓ | Document |
| FileId | UNIQUEIDENTIFIER | ✓ | ✓ | File |

## [DocumentType](./DocuTest.Data.Main.Generator/DocumentFile.sql)
| Name | Type | Required | PK | FK |
| ------------- | ------------- | ------------- | ------------- | ------------- |
| Id | UNIQUEIDENTIFIER | ✓ | ✓ |  |
| Name | NVARCHAR(255) | ✓ |  |  |

## [Metadata](./DocuTest.Data.Main.Generator/Metadata.sql)
| Name | Type | Required | PK | FK |
| ------------- | ------------- | ------------- | ------------- | ------------- |
| FileId | UNIQUEIDENTIFIER | ✓ | ✓ | File |
| Key | NVARCHAR(255) | ✓ | ✓ |  |
| Value | NVARCHAR(Max) | ✓ |  |  |

# Design Decisions
## Architecture and Structure
- A standard multi-layer architecture, utilizing an [API](DocuTest.Api/) layer, [Application](DocuTest.Application/) layer and [DAL](DocuTest.Data.Main.DAL/) layer;
- A [Shared](DocuTest.Shared/) project, defining the models used for cross-layer communication;
- An [SQL](DocuTest.Data.Main.Generator/) project, used to define the DB schema;
- A parallel [DataGenerator](DocuTest.Data.Main.DAL/Generators/DataGenerator.cs), used to populate 50 millions documents and up to 5 times as many files and metadata;

## Tools
- [NewtonsoftJson](https://www.newtonsoft.com/json) used for serialization. `System.Test.Json` is still not mature enough and has trouble deserializing the `byte[]` type;
- [Dapper](https://dapper-tutorial.net/dapper) used for its simplified interface, model binding, cancellation and parameter declaration capabilities;

## Data Access Mechanisms
- Every service acts as a UnitOfWork, taking care of opening and closing connections, starting transactions and logically encapsulating autonomous opperations;
- Every repository defines both the type and wording of the SQL query, does the data binding and adjacent table Join/Insert/Delete (when dealing with M:M or 1:M relationships for non-entity tables);

## Data Strategy
The problem is knowing whether a certain user has the ability to read or write a certain resource, based on the type of the resource itself. Since we have a dependency on the resource type itself, the out of the box role authentication mechanism of MVC will not suffice.

To solve the issue, a custom strategy framework is introduced:
- The [IDataStrategy](./DocuTest.Shared/Interfaces/IDataStrategy.cs)'s role is to define the interface for building an SQL expression as well as to be able to accept the entity its generalized for and decide whether the needed action is allowed;
- The abstract [SqlStrategy](./DocuTest.Shared/Strategies/SqlStrategy.cs) implements the [IDataStrategy](./DocuTest.Shared/Interfaces/IDataStrategy.cs), gathers the record requirements passed through the constructor and builds the SQL expression;
- The [IDocumentReadStrategy](./DocuTest.Application/Interfaces/IDocumentReadStrategy.cs) and [IDocumentWriteStrategy](./DocuTest.Application/Interfaces/IDocumentWriteStrategy.cs) define the specific entity action interface, allowing for multiple specific strategies to be implemented, based on varying criteria;
- The [DocumentRoleReadStrategy](./DocuTest.Application/Strategies/DocumentRoleReadStrategy.cs) and [DocumentRoleWriteStrategy](./DocuTest.Application/Strategies/DocumentRoleWriteStrategy.cs) define the record requirements based on the user's role information fetched from the [IUserContext](./DocuTest.Shared/Interfaces/IUserContext.cs);

Having this mechanism allows us to have different strategy definitions for every needed entity action as well as multiple implementations for every definition, based on the verying criteria we might have to make an access decision. In an architectural sense, it is the DAL's responsibility to utilize the strategies accordingly by using the generated SQL expressions for read actions or by checking the entities for eligibility, before write actions. It's the Service layer's responsibility to pick and pass the correct strategy, in line with its intended use - acting as a Business layer and orchestrator of rules and actions.

## Testing
- [Tested](DocuTest.Tests.Unit/Services/DocumentServiceTests.cs) the [DocumentService](DocuTest.Application/Services/DocumentService.cs) class with both success and fail scenarios, making sure all the repository and transaction methods are called correctly;

## Considered, but not implemented:
- Model validation;
- Global error handling and logging;
- Further optimization and SQL indexes;
- Using specialized models for Get/Insert/Update opperations;
- Introducing seperate Request/Response/Exchange models to handle API layer declarative functionality like serialization or data annotations for validation;
- Introducing sorting and paging for the `get-by-metadata` route;
- Getting a document will not return an existing document if the strategy does not allow fetching it. Having to actually check if the document exists in addition to whether the user can fetch it so that a more meaningfull exception is thrown produces a performance hit.