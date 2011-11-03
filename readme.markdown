AzureARR
========

A Web Role which will act as a sticky session load balancer for a given Role in Azure.

How it works
------------

In the solution are three projects.

- __Two10.Azure.Arr.Cloud__ The Cloud project which will deploy the solution to Azure.
- __Two10.Azure.Arr__ A Web Role which contains the ARR installation files, and code to configure ARR to load balance requests to your servers using client affinity (sticky sessions).
- __WebRole1__ An example web application, which displays the Azure the instance id, for you to check that the load balancing works. This should be replaced by your application.

