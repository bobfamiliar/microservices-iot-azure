#Microservices, IoT and Azure
###Date: 10.20.2015
###Version: v1.0.0
###Author: Bob Familiar
###URL: http://bobfamiliar.github.io/microservices-iot-azure/


----------


![Microservices, IoT and Azure][1]

----------

##Update - 04.09.2016

In order to support the velocity of development in Azure and Azure PowerShell, I have created a new content collection that is offered as a 5-module hands-on-lab that focuses on automation, microservices and IoT. The previous content collection for the book is located in the 'archive' folder. To leverage the new material, see the Lab Workbooks in the documentation folder.

##Overview
The book, Microservices, IoT and Azure, provides practical guidance for adopting a high velocity, continuous delivery process to create reliable, scalable, Software-as-a-Service (SaaS) solutions that are designed and built using a microservice architecture, deployed to the Azure cloud, and managed through automation. Microservices, IoT, and Azure offers software developers, architects, and operations engineers’ step-by-step directions for building cloud-native applications through code, script, exercises, and a working reference implementation.

This code repository is a reference implementation that accompanies the book Microservices, IoT and Azure by Bob Familiar and published by Apress Media. The book is available for purchase [here][2].

This hands-on training content provides foundational knowledge in how to architect and implement an IoT solution using Windows 10 Core IoT hardware devices and Azure IoT Hub and Stream Analytics. Both Device to Cloud and Cloud to Device communication patterns are covered.

At the conclusion of going through this workshop you will have provisioned an Azure environment using PowerShell that contains IoT Hub, DocumentDb, API Management, Storage, Service Bus, Stream Analytics Jobs and a Microservice for provisioning devices. You will also develop a Windows 10 Core IoT application that sends telemetry and receives incoming commands as well as develop a real-time dashboard that displays incoming telemetry and has the ability to send commands to the remote device. Device Provisioning, IoT Hub monitoring and techniques for dynamic business rules are covered.

The solution that you will build and deploy consists of the following components:

- Device: a Windows 10 IoT Core IoT solution that dynamically connects to IoT hub providing heartbeat and climate telemetry and processes several incoming commands. The device application will run on your local system or can be deployed to a Windows 10 Core IoT device
- Dashboard: a Windows 10 WPF application that displays registered devices, map location using Bing Maps, incoming device telemetry and alarms
- Provision API: A ReST API the provides end points for device registration with IoT Hub and DocumentDb and device manifest lookup via unique serial number. The Dashboard application registered devise and the Device application uses the API to retrieve its manifest
- IoT Hub Listener: a debugging utility that provides visibility to messages arriving from the device

And the following Azure Services

- API Management – provides proxy, policy injection and developer registration services for ReST APIs
- Service Bus Namespace – two queues are defined, one that is a target for all incoming messages, the other will have receive messages that contain data that is an alarm state, an out of range value
- IoT Hub – IoT Hub provides device registration, incoming telemetry at scale and cloud to device message services
- Stream Analytics Job – two solution uses two Stream Analytics jobs, one that handles all incoming messages routing them to one queue and the other identifies alarm states and routs those messages to another queue

##Requirements

 - Windows 10
 - [Azure Account][4]
 - [Visual Studio 2015][5]
 - [Visual Studio 2015 Update 1][6] 
 - [PowerShell 5][7]
 - [Azure SDK 2.8 and Azure PowerShell 1.2.1][8]
 - [Windows 10 Core IoT Templates][9] 
 - Go to the [Bing Maps Portal][10], sign in and request a developer key
 - Install the [Bing Maps WPF Control][11]

See the [Wiki][3] for release notes and known issues

## Chapters 
**Chapters 1: From Monolithic to Microservice** - Shifting demographics and competitive pressure on business to drive impact at velocity is requiring us to evolve our approach to how we develop, deploy, and support our software products. This chapter lays out a roadmap to evolve not only application architecture but also process and organization.

**Chapters 2: What Is a Microservice?** - This chapter provides a working definition of microservices and details the benefits as well as the challenges to evolving to this architecture pattern.

**Chapters 3: Microservice Architecture** - Traditionally, we have used separation of concerns, a design principle for separating implementation into distinct layers in order to define horizontal seams in our application architecture. Microservice architecture applies separation of concern to define vertical seams that define their isolation and autonomous nature. This chapter will compare and contrast microservice architecture with traditional layered architecture.

**Chapters 4: Azure – A Microservice Platform** - The Azure platform exudes characteristics of microservices. This chapter examines several Azure services to identify common patterns of services that are designed and implemented using microservices. Storage, SQL Database, DocumentDb, Redis Cache, Service Bus, API management, and app containers are reviewed.

**Chapters 5: Automation** - Automation is the key to being able to evolve to a continuous delivery approach and realize the benefits of SaaS. This chapter outlines a framework for defining and organizing your automation process and takes you through 10 exercises that will provision, build, and deploy the reference implementation using PowerShell.

**Chapters 6: Microservice Reference Implementation** - The epic story of Home Biomedical, a wholly owned subsidiary of LooksFamiliar, Inc., is detailed, and the implementation details of the reference microservices are revealed. The common libraries for ReST calls and DocumentDb and Redis Cache for data access are reviewed. Designing for both public and management APIs is discussed along with the implementation details for the model, interface, service, API, SDK, and console components.

**Chapters 7: IoT and Microservices** - IoT is becoming a more common solution pattern as we learn to incorporate streaming data into our solutions. This chapter outlines the capabilities needed to realize an IoT solution and maps them to the Azure IoT stack. IoT Hub, IoT Suite, Event Hub, and Stream Analytics are detailed, as well as how to use Event Hub, Cloud Services, and Notification Hub to support mobile alerts. A working example of data visualization using a JavaScript client along with SignalR, ReST, and SQL Database is reviewed.

**Chapters 8: Service Fabric** - Service Fabric is the microservice management, runtime, and infrastructure that Microsoft uses to build, deploy, and manage their own first-class cloud services such as SQL Database, DocumentDb, Bing Cortana, Halo Online, Skype for Business, In Tune, Event Hubs, and many others. This chapter provides a primer and demonstrates Service Fabric by migrating one of the Web API microservices to Service Fabric.

----------

  [1]: http://bobfamiliar.azurewebsites.net/wp-content/uploads/2015/10/bookcover-small.jpg
  [2]: http://amzn.to/1RFjiUW "Amazon"
  [3]: https://github.com/bobfamiliar/microservices-iot-azure/wiki
[4]: https://azure.microsoft.com/en-us/ 
[5]: https://www.visualstudio.com/en-us/products/vs-2015-product-editions.aspx 
[6]: https://www.visualstudio.com/en-us/news/vs2015-update1-vs.aspx 
[7]: https://www.microsoft.com/en-us/download/details.aspx?id=50395
[8]: https://azure.microsoft.com/en-us/downloads/
[9]: https://visualstudiogallery.msdn.microsoft.com/55b357e1-a533-43ad-82a5-a88ac4b01dec
[10]: https://www.bingmapsportal.com 
[11]: https://www.microsoft.com/en-us/download/details.aspx?id=27165 


