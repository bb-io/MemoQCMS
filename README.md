# Blackbird.io memoQ CMS

Blackbird is the new automation backbone for the language technology industry. Blackbird provides enterprise-scale automation and orchestration with a simple no-code/low-code platform. Blackbird enables ambitious organizations to identify, vet and automate as many processes as possible. Not just localization workflows, but any business and IT process. This repository represents an application that is deployable on Blackbird and usable inside the workflow editor.

## Introduction

<!-- begin docs -->

MemoQ CMS is a platform that streamlines workflows by providing efficient job submission, processing, and delivery within the memoQ Server environment, enhancing overall translation management capabilities.

## Before setting up

Before you can connect you need to make sure that:

- You have [memoQ installed](https://www.memoq.com/downloads).
- You have created connection and obtained your _Base URL_ and _Connection key_. Information about connection establishment can be found in the [_memoQ Server CMS API documentation_](https://docs.memoq.com/current/api-docs/cmsapi/CMS%20Gateway%20REST%20API%20v2.00.pdf?_gl=1*nqna0h*_ga*ODg3NDQ5Njc0LjE3MDExNjIwMjY.*_ga_HHK0YX9VVW*MTcwMTI0MzUxMS4zLjAuMTcwMTI0MzUxMS4wLjAuMA..*_ga_TVK7MSKW78*MTcwMTI0MzUxMS4zLjAuMTcwMTI0MzUyMC4wLjAuMA..*_gcl_au*MTM3Njk1OTc2NC4xNzAxMTYyMDI1#page=13&zoom=100,90,94) under the _2.3.1. CMS connection lifecycle_ and _2.3.2. CMS connection management_ sections.

## Actions

### Orders 

- **List orders**. Orders can be optionally filtered using input parameters.
- **Get order**.
- **Create order**.
- **Commit order** changes the status of an order to _Committed_ which means that all jobs of the order have been submitted.

## Feedback

Do you want to use this app or do you have feedback on our implementation? Reach out to us using the [established channels](https://www.blackbird.io/) or create an issue.

<!-- end docs -->