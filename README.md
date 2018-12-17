# Community System Job Cleanup/Management

![alt text](https://github.com/colathro/CRMAsyncCleanup/blob/master/Dynamics-365-Async-Cleanup/Reference%20Guide/CurrentVersionScreenshot.jpeg?raw=true)

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
