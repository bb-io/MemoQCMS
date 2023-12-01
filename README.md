# Blackbird.io memoQ CMS

Blackbird is the new automation backbone for the language technology industry. Blackbird provides enterprise-scale automation and orchestration with a simple no-code/low-code platform. Blackbird enables ambitious organizations to identify, vet and automate as many processes as possible. Not just localization workflows, but any business and IT process. This repository represents an application that is deployable on Blackbird and usable inside the workflow editor.

## Introduction

<!-- begin docs -->

MemoQ CMS is a platform that streamlines workflows by providing efficient job submission, processing, and delivery within the memoQ Server environment, enhancing overall translation management capabilities.

## Before setting up

Before you can connect you need to make sure that:

- You have [memoQ installed](https://www.memoq.com/downloads).
- You have created connection and obtained your _Base URL_ and _Connection key_. Information about connection establishment can be found in the [_memoQ Server CMS API documentation_](https://docs.memoq.com/current/api-docs/cmsapi/CMS%20Gateway%20REST%20API%20v2.00.pdf?_gl=1*nqna0h*_ga*ODg3NDQ5Njc0LjE3MDExNjIwMjY.*_ga_HHK0YX9VVW*MTcwMTI0MzUxMS4zLjAuMTcwMTI0MzUxMS4wLjAuMA..*_ga_TVK7MSKW78*MTcwMTI0MzUxMS4zLjAuMTcwMTI0MzUyMC4wLjAuMA..*_gcl_au*MTM3Njk1OTc2NC4xNzAxMTYyMDI1#page=13&zoom=100,90,94) under the _2.3.1. CMS connection lifecycle_ and _2.3.2. CMS connection management_ sections.

## Connecting

1. Navigate to apps and search for memoQ CMS. If you cannot find memoQ CMS then click _Add App_ in the top right corner, select memoQ CMS and add the app to your Blackbird environment.
2. Click _Add Connection_.
3. Name your connection for future reference e.g. 'My organization'.
4. Fill in the base URL and connection key you obtained earlier. You can also copy the connection information. To do this, navigate to _Server Administrator_ -> _CMS connections_ and click on _Copy client configuration information to the clipboard_.
5. Click _Connect_.
6. Confirm that the connection has appeared and the status is _Connected_.

![Connecting](image/connecting.png)

## Actions

### Orders

- **List orders**. Orders can be optionally filtered using input parameters.
- **Get order**.
- **Create order**.
- **Commit order** changes the status of an order to _Committed_ which means that all jobs of the order have been submitted.

### Jobs

- **List jobs** returns the translation jobs of the specified order.
- **Get job**.
- **Create job** creates a new translation job in the specified order. Note that you cannot add a new job to an order with a _Committed_ status.
- **Download translation** downloads the translation for the specified job. The job must be in the _In Delivery_ or _Delivered_ state before the translation can be downloaded.
- **Deliver job** changes the status of a job to _Delivered_.

## Feedback

Do you want to use this app or do you have feedback on our implementation? Reach out to us using the [established channels](https://www.blackbird.io/) or create an issue.

<!-- end docs -->