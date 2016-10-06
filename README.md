# Moving a Node.js API with MongoDb backend to Microsoft Azure in 15 minutes

This is a quick demo that I put together in order to showcase how easy it is to take an on-premises application written in [Node.js](https://nodejs.org/) (with a [MongoDb](https://www.mongodb.com/) data store) and host it into [Microsoft Azure](https://azure.microsoft.com/)'s PaaS (Platform as a Service) with no servers to manage, and turn-key scalability, security and management.

You will find everything you need to replicate this demo in the [GitHub repo](https://github.com/neaorin/NodeMongoDemo). Once you have done the initial setup, you should be able to perform the actual migration to the cloud **in about 15 minutes**. This also involves creating the required Azure services on the spot and deploying code and data.

## Table of Contents

1. [Intro](#ch1)

2. [What you'll need](#ch2)

3. [Set up the application locally](#ch3)

4. [Migrate the MongoDb database to Azure DocumentDB](#ch4)

5. [Migrate the Node.js application to Azure App Service](#ch5)

6. [Next steps](#ch6)

### <a name="ch1"/>1. Intro

If you're writing mobile applications, then you most certainly need a place to store data centrally, do authentication, authorization, send push notifications to the users and so on. Most people build a *backend* for their app for this very reason. 

Minimally, the backend consists of an API (typically REST over HTTPS) which your mobile app talks to (usually by using JSON or XML), which can be built on a number of technologies - Node.js, ASP.NET, Java, PHP or Python are all widely used.

You also need some sort of data store. The data store can be relational (MySQL, Microsoft SQL Server, or others) or non-relational (NoSQL databases like MongoDb are very popular).

![](images/mobileapp1.png)

Now, when you take this server environment into the Azure cloud, you have a couple of options. You could decide to use [Virtual Machines](https://azure.microsoft.com/en-us/services/virtual-machines/) which give you the flexibility of being able to do everything you would do on a virtual private server, but that comes at the expense of additional management - you're responsible for securing the box, do load balancing, scaling in and out, and so on.

A better option for this scenario - and many others - would be to use Microsoft's own managed services, which come by default with their own SLA, automated patching, far easier scaling options etc. I am talking specifically about [App Service Mobile Apps](https://azure.microsoft.com/en-us/services/app-service/mobile/) and [DocumentDB](https://azure.microsoft.com/en-us/services/DocumentDB/) respectively.

![](images/mobileapp2.png)

The former is a fully managed service for mobile API backends, and comes built-in with [really useful features](https://azure.microsoft.com/en-us/documentation/learning-paths/appservice-mobileapps/) such as authentication, offline sync, push notifications, auto-scaling, backup etc.

The latter is a highly distributed (up to petabytes) NoSQL document database that also comes with [protocol support for MongoDb](https://azure.microsoft.com/en-us/documentation/articles/DocumentDB-protocol-mongodb/), so you don't need to modify an existing app targeting MongoDb.

The rest of this walkthrough describes the process of migrating a locally-hosted Node.js + MongoDb environment into App Service Mobile Apps + DocumentDB.

### <a name="ch2"/>2. What you'll need

1. A PC or laptop with Windows 8.1, Windows 10 or MacOS X.
2. An Azure subscription. You can get $25 worth of Azure each month for free by signing up for [Visual Studio Dev Essentials](https://www.visualstudio.com/dev-essentials/). Or, you can just get a [free Azure trial](https://azure.microsoft.com/en-us/free/).
2. Visual Studio 2015 ([free Community Edition download](https://www.visualstudio.com/downloads/)) with the Xamarin tools installed if you're using Windows, or [Xamarin Studio](https://www.xamarin.com/studio) for Mac, also a free download.
3. [Node.js](https://nodejs.org/) installed locally.
4. [MongoDb](https://www.mongodb.com/) installed and running locally (Windows tutorial [here](https://docs.mongodb.com/manual/tutorial/install-mongodb-on-windows/)).
5. A MongoDb management tool, this tutorial uses the free edition of [MongoChef](http://3t.io/mongochef/).
7. An FTP client, this tutorial uses [FileZilla](https://filezilla-project.org/).
6. (Recommended) A tool like [Postman](https://www.getpostman.com/) to debug and test if you encounter any issues. 

### <a name="ch3"/>3. Set up the application locally

#### 3.1 MongoDb database

First things first - clone or download the [GitHub repo](https://github.com/neaorin/NodeMongoDemo) to a local folder.

Download and install MongoDb, then run the server process (docs for [Windows](https://docs.mongodb.com/manual/tutorial/install-mongodb-on-windows/), [OS X](https://docs.mongodb.com/manual/tutorial/install-mongodb-on-os-x/)).

Use [MongoChef](http://3t.io/mongochef/) or a similar tool to connect to the MongoDb local instance. If you used the default settings, the server should run on port **27017**.

Once you're there, create a database named **tododb**.

![](images/mongo01.png)

![](images/mongo02.png)

Inside the **tododb** database, create a collection called **TodoItems**.

![](images/mongo03.png)

![](images/mongo04.png)

Now it's time to insert some data into the collection. We can use MongoChef's IntelliShell feature and the **mongodb/insertdata.js** script you can find in this GitHub repo to insert several items into the newly created collection.

![](images/mongo05.png)

![](images/mongo06.png)

For reference, here is the script used in these screenshots.

```js
db.TodoItems.insertMany(
[
	  { name: "Send welcome emails", notes: "by tuesday", done: false },
	  { name: "Talk to partners", notes: "", done: false },
	  { name: "Go to the mountains", notes: "", done: false },
	  { name: "Read a new book", notes: "", done: false },
	  { name: "Run a marathon", notes: "", done: true },
	  { name: "Pick up package", notes: "", done: true },
	  { name: "Do a presentation", notes: "in November", done: false }
]
);
``` 

After you've inserted the new documents, check that they are actually inside the **TodoItems** collection.

![](images/mongo07.png)

#### 3.2 Node.js API

Download and install [Node.js](https://nodejs.org/); once you have it installed, open a command prompt and navigate to the Node.js app which is located inside the **TodoApi** folder.

```
> cd <path_to_local_copy>\TodoApi
```

Make sure you have the Node Package Manager (`npm`) v3.x or newer installed. You can do this by running this command and checking the version: 

```
> npm -v 
3.10.8
```

> NOTE: upgrade your `npm` if you've got a version older than 3.x, as older versions of `npm` can run into trouble with long file paths on Windows.

Open the **TodoApi/api/controllers/db.json** file in an editor, and check that the MongoDb connection string points to your local server and database:

```
{
    "db_url": "mongodb://localhost:27015/tododb"
}
```  

Download the project's npm dependencies:

```
> npm install
```

Run the app from the command prompt:

```
> node app.js
App is running on port 8080...
```

Now let's test that the application works and can read the data from the database. Use Postman or any web browser to browse to *http://localhost:8080/todo* and check that the app returns data.

![](images/postman01.png)

#### 3.3 The mobile application

![](images/mobile01.png)

Finally, you should set up and run the mobile application, which will connect to the Node API to collect and display data.

The demo app included in this repo is a Xamarin Forms application - based on an [official Xamarin sample](https://developer.xamarin.com/samples-all/) - which can be run on Windows 10, Android or iOS. 

The screenshot above shows the Windows 10 app running side by side with the Android app - the latter running inside the [Visual Studio Emulator for Android](https://www.visualstudio.com/vs/msft-android-emulator/).

In order to build and run the app on Windows 10, do the following:

1. Open the **TodoMobileApp/PCL/TodoPCL.sln** solution file with Visual Studio 2015.
2. Right click the **Todo.UWP** project and click **Build**.

![](images/uwp01.png)

3. Once the Build is done, right click the same project and click **Deploy**.
4. If the deployment is successful, the app should be installed locally as a Store app. Therefore, you can hit the Windows Start button and search for **Todo.UWP**

![](images/uwp02.png)

5. Start the app, then verify that the API url is pointing to the Node.js app, should be *http://localhost:8080/todo*.

![](images/uwp03.png)

Verify that the app displays the data, and you can add, edit, delete items.
 
![](images/uwp04.png)


If you're on a Mac, you should do the following to run the iOS version:

1. Open the **TodoPCL** solution in Xamarin Studio.

2. Select the **Todo.iOS** project, then click the Play arrow in the toolbar to start the iOS emulator.

![](images/mac01.png)

![](images/mac02.png)

### <a name="ch4"/>4. Migrate the MongoDb database to Azure DocumentDB

Navigate to the Azure Portal at [https://portal.azure.com](https://portal.azure.com)

In the portal, click **New / Databases / DocumentDB (NoSQL) - Protocol support for MongoDB (preview)**

> NOTE: this feature will exit preview in the future, and the item will be folded under **DocumentDB (NoSQL)**.

![](images/docdb01.png)

Enter a name for your DocumentDB server, select or enter a new Resource Group and select an Azure Region (below are just some examples).

![](images/docdb02.png)

Hit **Create** and wait a couple of minutes for the service to be provisioned.

Once it's done, navigate to it, for example by using the **All Resources** button in the left pane of the Azure Portal.

In the DocumentDb server pane, go to **Connection String** and copy all the values for *Host*, *Port*, *Username*, *Password* and *Connection String*.

![](images/docdb03.png)

Now use the Host, Port, Username and Password to connect to the server using MongoChef, the sme tool you used to connect to your local MongoDb instance.

Create a new connection from MongoChef, and enter the server, host, username and password. Make sure you also check the option to use SSL as shown below.

![](images/docdb04.png)

![](images/docdb05.png)

![](images/docdb06.png)

Create a **tododb** database and a **TodoItems** collection in DocumentDB, exactly the same as you did earlier locally.

![](images/mongo02.png)

![](images/mongo04.png)

Now it's time to get some data into DocumentDB. While you *could* use the same *mongodb/insertdata.js* script you used earlier, this time we're gonna use MongoChef's own Export/Import feature to simulate a real-world lift and shift of data from on-premises to the cloud.

In MongoChef, expand the local Mongodb instance, right click the **TodoItems** collection and hit **Export Collection...**

![](images/docdb07.png)

Follow the wizard to select the **TodoItems** on the DocumentDB cloud instance as the destination.

![](images/docdb08.png)

![](images/docdb09.png)

![](images/docdb10.png)

![](images/docdb11.png)

Once the wizard finishes, you should have your tasks data inside the DocumentDB service!

You can test by pointing the local Node.js API at the new cloud database. Since DocumentDB has protocol support for MongoDb, we only need to change the connection string inside the **db.json** file. 

Open the file again, and paste the DocumentDB connection string from the Azure Portal, **making sure to add the name of the database tododb** at the end of the path, before the *?ssl=true* query string:

```
{
    "db_url": "mongodb://sorinnodedemo:longpwd...@sorinnodedemo.documents.azure.com:10250/tododb?ssl=true"
}
```   

> NOTE: make sure to add the bolded part towards the end of the string, to signal the fact that you're using the **tododb** database:

> ....documents.azure.com:10250/**tododb**?ssl=true

Save your changes to the **db.json** file, then go to the console and restart the node process.

```
^C
> node app.js
App is running on port 8080...
```

Now if you run Postman or the UWP client app again, they should still work just as they did before.

### <a name="ch5"/>5. Migrate the Node.js application to Azure App Service

Now we're gonna take the Node.js API and put that into the Azure App Service container.

From the Azure Portal, click New / Web + mobile / Mobile App

![](images/appservice01.png)

Enter the app name, the Resource Group (you can use the group you created earlier) and select or create an App Service Plan.

![](images/appservice02.png)

For a new App Service Plan, click the Pricing tier option, then click View all to see all the options and select the Free tier (or whatever tier you're comfortable with). 

![](images/appservice03.png) 

Hit **Create** and wait about a minute or so for the App Service to be provisioned.

Once it's done, navigate to the new service, and go to **Deployment credentials**. 

![](images/appservice04.png) 

Enter a new FTP username and password, and hit **Save**. You'll use these credentials in the next step to transfer your API to the service.

Now go to the Overview pane, note and copy the FTPS username and host.

![](images/appservice05.png) 

Open your FTP client and connect to the server. Use the full username and password from the Overview pane.

Inside the FTP server, navigate to the **/site/wwwroot** folder and copy all the files from the **TodoApi** local folder (same folder you ran `node app.js` in).

Here is a screenshot of what it would look like in FileZilla.

![](images/appservice06.png) 

Wait for a few minutes for the file copying to finish.

> **Wait, so how does the App Service know to run Node.js for my app?**

> One of the files in the **TodoApi** folder is named *Web.config*. This file is not used by Node.js locally; it is the [Internet Information Server](https://www.iis.net/) (IIS) configuration file used by the App Service to run node for requests getting to that web server. 

```xml
<configuration>
  <system.webServer>
    ...
    <handlers>
      <!-- Indicates that the server.js file is a node.js site to be handled by the iisnode module -->
      <add name="iisnode" path="app.js" verb="*" modules="iisnode"/>
    </handlers>
      ...
```

> There are similar examples in the Azure documentation for using the App Service to host websites written in [PHP](https://azure.microsoft.com/en-us/develop/php/), [Java](https://azure.microsoft.com/en-us/develop/java/), [Python](https://azure.microsoft.com/en-us/develop/python/), [.NET](https://azure.microsoft.com/en-us/develop/net/) and others.

Once the file copy is done, the API should be up and running. 

You can test by using Postman or a browser to open the API URL. That can be found inside the **Overview** pane as well.

![](images/appservice07.png) 

For example, in Postman it should look like this:

![](images/appservice08.png) 

And from the mobile app itself (note that you can replace HTTP with HTTPS and it will work, so SSL is built-in):

![](images/appservice09.png) 

![](images/uwp04.png)

### <a name="ch6"/>6. Next steps

So now you've got a nice REST API with a database backing it up, all up and running in Azure. What are some other things you can explore? How about these:

- [Scale out the app](https://azure.microsoft.com/en-us/documentation/articles/web-sites-scale/) by hand or automatically, so you're always ready for new users, and only pay for what you actually use
- [Send push notifications](https://azure.microsoft.com/en-us/documentation/articles/notification-hubs-push-notification-overview/) to iOS, Android and Windows in a unified way
- [Enable authentication](https://azure.microsoft.com/en-gb/documentation/articles/app-service-mobile-ios-get-started-users/) with Facebook, Google, Twitter; or use [Azure AD](https://azure.microsoft.com/en-gb/services/active-directory-b2c/) to have your own managed, secure user store and never have to deal with passwords and registration again.
- [Enable offline sync](https://azure.microsoft.com/en-gb/documentation/articles/app-service-mobile-offline-data-sync/) and have your app work flawlessly when the user is offline, then easily sync up when they get back online.
- [Get application telemetry](https://azure.microsoft.com/en-us/services/application-insights/) from App Insights so you know what your users are doing and when they get in trouble.
