- title : Delving into Documents with Data Subscriptions
- description : 
- author : Kijana Woodard
- theme : night
- transition : default

***

# Delving into Documents with Data Subscriptions

## RavenConf 2016
May 24, 2016

###Kijana Woodard

***

##Slides
http://kijanawoodard.github.io/Talk-RavenConf-2016

***
## About Me

Live in Dallas, TX

Programming professionally since 1996

.Net since 1.0

Contracting since 2010

***
## On the web

@kijanawoodard

http://kijanawoodard.com

RavenDB forum

NServiceBus forum

DDD/CQRS forum

API Craft forum

***

## Using Data Subscriptions?

' How many using?
' Not much forum activity

***

## A Solution is Problem Substitution

***

## System components should be <del>Composable</del> Disposable.

' Organize applications

***

## What are Data Subscriptions?

Data subscription provide a reliable and handy way to retrieve documents from the database for processing purposes by application jobs.

' Persistent
' Etag
' At least once

***

## Using Data Subscriptions

- Create
- Open 
- Subscribe
- Release
- List
- Delete

***

## Using Data Subscriptions

- Create
- Open 
- Subscribe
- Release
- List
- **Delete**

---

## Using Data Subscriptions

- Create
- Open 
- Subscribe
- Release
- **List**
- <del>Delete</del>

---

## Using Data Subscriptions

- **Create**
- Open 
- Subscribe
- Release
- <del>List</del>
- <del>Delete</del>

---

## Using Data Subscriptions

- <del>Create</del>
- Open 
- Subscribe
- **Release**
- <del>List</del>
- <del>Delete</del>

' Single consumer

---

## Using Data Subscriptions

- <del>Create</del>
- **Open** 
- Subscribe
- <del>Release</del>
- <del>List</del>
- <del>Delete</del>

---

## Using Data Subscriptions

- <del>Create</del>
- <del>Open</del>
- **Subscribe**
- <del>Release</del>
- <del>List</del>
- <del>Delete</del>

' Install-Package RX-Main

---

## Using Data Subscriptions

- <del>Create</del>
- <del>Open</del>
- <del>Subscribe</del>
- <del>Release</del>
- <del>List</del>
- <del>Delete</del>

***

## Alternatives to Data Subscriptions?

***

## Alternatives to Data Subscriptions?

- Query with Paging
- Stream API
- Changes API
- Patch
- Message queues

***

## What's Missing?

' Etag + document id in subscription info document
' Write assurance

***

## Questions?