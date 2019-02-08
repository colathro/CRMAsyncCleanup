# Community System Job Cleanup/Management

![alt text](https://github.com/colathro/Dynamics-CRM-Community-System-Job-Cleanup/blob/master/Dynamics-365-Async-Cleanup/Reference%20Guide/CurrentVersionScreenshot.JPG?raw=true)

## Usage:
### 1. Use your instance's advanced find to build a query which returns your desired records to operate on. (Test)
### 2. Download Fetch XML and Open.
### 3. Trim off unneccesary columns and sorts for performance - Only column needed is asyncoperationid
From: 
```
<fetch version="1.0" output-format="xml-platform" mapping="logical" distinct="false">
<entity name="asyncoperation">
<attribute name="asyncoperationid"/>
<attribute name="name"/>
<attribute name="regardingobjectid"/>
<attribute name="operationtype"/>
<attribute name="statuscode"/>
<attribute name="ownerid"/>
<attribute name="startedon"/>
<attribute name="statecode"/>
<order attribute="startedon" descending="true"/>
<filter type="and">
<condition attribute="createdon" operator="on-or-before" value="2019-02-01"/>
<condition attribute="statecode" operator="eq" value="3"/>
</filter>
</entity>
</fetch>
```

To: 
```
<fetch version="1.0" output-format="xml-platform" mapping="logical" distinct="false">
<entity name="asyncoperation">
<attribute name="asyncoperationid"/>
<filter type="and">
<condition attribute="createdon" operator="on-or-before" value="2019-02-01"/>
<condition attribute="statecode" operator="eq" value="3"/>
</filter>
</entity>
</fetch>
```

### 4. Login to the tool and paste in your edited fetch xml.
### 5. Set the batch size (Recommended 250 for best peformance) and operation (Delete or Cancel)
### 6. Run and monitor!

## Utilized Packages/Resources:
1. [MaterialDesignXaml](http://materialdesigninxaml.net/)
2. [CRMSdk Core Assemblies](https://www.nuget.org/packages/Microsoft.CrmSdk.CoreAssemblies/)
3. [Xrm Tooling Core Assemblies](https://www.nuget.org/packages/Microsoft.CrmSdk.XrmTooling.CoreAssembly/)
4. [Xrm Tooling Wpf Controls](https://www.nuget.org/packages/Microsoft.CrmSdk.XrmTooling.WpfControls/)


## Core Values:
1. Simple Implementations
2. Verbose Variable Naming
3. Excessive Commenting :+1:


## Core Values Explained:
1. Simple implementations to allow new developers easy examples and practical implentations to build off. Complexity != Solid Implmentation

2. Verbose variable naming allows for more readable and beginner friendly experience. Short variable names are :sunglasses:, but impractical.

3. Excessive commenting should allow for better explaination throughout the app and the ability to easily identify features/processes.

![alt text](https://github.com/colathro/CRMAsyncCleanup/blob/master/Dynamics-365-Async-Cleanup/Reference%20Guide/AdvancedFind.png?raw=true)

## TODO:
1. Fix Multithreading - assumed OrganizationServiceProxy could be called on different threads but internal lock due to WCF prevents this.
2. Better rate tracking
3. Add Logging
