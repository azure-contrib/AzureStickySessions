# AzureStickySessions

A Web Role which will act as a sticky session load balancer for a given Role in Azure.

## How it works

In the solution are three projects.

* __Two10.Azure.Arr.Cloud__ The Cloud project which will deploy the solution to Azure.
* __Two10.Azure.Arr__ A Web Role which contains the ARR installation files, and code to configure ARR to load balance requests to your servers using client affinity (sticky sessions).
* __WebRole1__ An example web application, which displays the Azure the instance id, for you to check that the load balancing works. This should be replaced by your application.

Requests coming in to Azure will be handled by one of the Two10.Azure.Arr instances. If this is the first request made by the client, they will be directed to a WebRole1 instance using a round robin algorithm. A cooking will be written, which will provide a hint in subsequent requests, so the client is redirected to the same instance (i.e. a sticky session).

The Two10.Azure.Arr application continually queries the instances in your deployment belonging to the WebRole1 role. These instances will be added to a web farm configured in IIS on this role. Instances that are removed from Azure will likewise be removed from the farm.

## Configuration

Two10.Azure.Arr has the following settings:

* __RoleName__: The name of the role you wish to load balance to (i.e. WebRole1). This is currently hard coded in the WebRole class.

The role being load balanced much have an internal endpoint configured, called 'Internal' (case sensitive).

## License

MIT
