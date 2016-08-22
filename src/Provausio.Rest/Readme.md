[![Build status](https://ci.appveyor.com/api/projects/status/fthf3c6f1mrm5fri?svg=true)](https://ci.appveyor.com/project/JeremyStafford/provausio-pqtm5)

[Nuget Package](https://www.nuget.org/packages/Provausio.Rest.Client)


# RestClient
A rest client with a fluid uri builder. Probably the most tedious thing about making HTTP calls is constructing the client. There are many optional steps to be taken, some by setting properties others by calling methods, etc. This seemed like a perfect opportunity for a builder. The builder pattern allows you to build something in optional steps as well as pass the client around your app to be built in steps in different places. This libary simply provides a builder implementation on the client itself.

## A simple get request

``` csharp
// http://www.google.com
var client = new RestClient();
client
  .WithScheme(Scheme.Http)
  .WithHost("www.google.com");

HttpResponseMessage response = await client.GetAsync();
```

## Adding a path the requested host

``` csharp
// https://www.microsoftstore.com/store/msusa/en_US/
var client = new RestClient();
client
  .WithScheme(Scheme.Https)
  .WithHost("www.microsoftstore.com")
  .WithPath("store/msusa/en_US/");

  HttpResponseMessage response = await client.GetAsync();
```

## Adding a key-value pair as part of the path
### This isn't technically any different than manually setting the path. However, this could be useful in some situations to look at it in RESTful terms.

```csharp
// https://www.microsoftstore.com/store/msusa/en_US/pdp/productID.3244388600
var client = new RestClient();
client
  .WithScheme(Shceme.Https)
  .WithHost("www.microsoftstore.com")
  .WithPath("store/msusa/en_US")
  .WithSegmentPair("pdp", "productID.3244388600");

HttpResponseMessage response = await client.GetAsync();
```

## Adding Query parameters
### These can be added as an object, which will reflect public properties into parameters. You can also provide a collection of KeyValuePair

``` csharp
// https://www.microsoftstore.com/store/msusa/en_US/pdp/productID.3244388600?filter1=value1&filter2=value2
var client = new RestClient();
client
  .WithScheme(Shceme.Https)
  .WithHost("www.microsoftstore.com")
  .WithPath("store/msusa/en_US")
  .WithSegmentPair("pdp", "productID.3244388600")
  .WithQueryStringParameters(new { filter1 = "value1", filter2 = "value2" });

HttpResponseMessage response = await client.GetAsync();
```

## Adding a body (for POST and PUT)
### Simply provide an instance of `HttpContent`. Some additional subclasses are provided.

``` csharp
// Requesting with a json body (POST or PUT) to https://api.myservice.com/api/users/12345
var client = new RestClient();
client
  .WithScheme(Scheme.Https)
  .WithHost("api.myservice.com")
  .WithPath("api")
  .WithSegmentPair("users", "12345");

HttpContent content = new StringContent(myJsonstring, Encoding.UTF8, "application/json");
HttpResponseMessage response = await client.Put(content);


// Requesting with an object that will be serialized to a form post (key value pairs) to https://api.myservice.com/api/users/12345
var client = new RestClient()
  .WithScheme(Scheme.Https)
  .WithHost("api.myservice.com")
  .WithPath("api")
  .WithSegmentPair("users", "12345")
  .AsClient();

HttpContent content = new Provausio.Rest.Client.ContentType.FormUrlEncodedContent(new { FirstName = "Jon", LastName = "Snow" });
HttpResponseMessage response = await client.Put(content);
```
