Walmart-SDK.NET
===============

.NET SDK for the Walmart Open API (https://developer.walmartlabs.com)

(The nuget package for this project is located at: https://github.com/NextGenWebStudios/Walmart-SDK.NET-NuGet)

## Code Example
```csharp

// Create the Walmart client
Walmart.SDK.Core.WalmartClient client = new Walmart.SDK.Core.WalmartClient("<api_key>");

// Get all categories
client.GetCategories((category) =>
{
    client.GetProducts(category.Id, (product) =>
    {
        // Do something!
        return true; // Keep iterating over products, return false to stop
    });

    return true; // Keep iterating over categories, return false to stop
});

```