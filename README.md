# Blackbird.io Strapi

Blackbird is the new automation backbone for the language technology industry. Blackbird provides enterprise-scale automation and orchestration with a simple no-code/low-code platform. Blackbird enables ambitious organizations to identify, vet and automate as many processes as possible. Not just localization workflows, but any business and IT process. This repository represents an application that is deployable on Blackbird and usable inside the workflow editor.

## Introduction

<!-- begin docs -->

Strapi is the next-gen headless CMS, open-source, JavaScript/TypeScript, enabling content-rich experiences to be created, managed and exposed to any digital device.

## Localization concept

In Strapi, localization is referred to as `internationalization` and is managed through the `i18n` plugin. To configure languages in your Strapi instance:

1. Navigate to `Settings > Internationalization`
2. Add the languages you want to support
3. Set your default language

When working with content types, you must explicitly enable localization for specific fields:
1. Go to the content type editor
2. Open the 'Advanced Settings' tab for the desired field
3. Enable the localization option

This configuration is crucial because without properly localizing fields, any translations uploaded will update content across all languages instead of only the selected language.

![Strapi i18n settings](docs/images/localization_field.png)

## Before setting up

Before you connect your Strapi instance to Blackbird, make sure you have the following:

- A Strapi instance running on a public accessible URL. 
- API token for the Strapi instance. You can create an API token in the Strapi Settings > API Tokens section. Make sure to give the token the necessary permissions to access the content you want to use in Blackbird. Typically we reommend using the 'Full access' token type. Also, setup the token duration to be 'Unlimited' to avoid having to update the token in Blackbird every time it expires.
- Installed the `i18n` plugin in your Strapi instance. This is required for localization support.

## Connecting

1. Navigate to apps and search for **Strapi**
2. Click _Add Connection_
3. Name your connection for future reference e.g., 'My Strapi'
4. Fill in the following fields:
   - **Base URL**: Your AEM base URL (e.g., `https://my-strapi-instance.com`)
   - **API Token**: Your Strapi API token (see the previous section)
5. Click _Connect_
6. Confirm that the connection has appeared and the status is _Connected_

![connection](docs/images/connection.png)

> Connection validation is done by fetching the all available locales from the Strapi instance using `/api/i18n/locales` endpoint. So, it's necessary to have the `i18n` plugin installed and configured in your Strapi instance.

## Actions

- **Search content**: Returns a list of content based on specified inputs. Only for collection types content types.
- **Download content**: Downloads content by ID as an HTML file. By default, downloads the published version.
- **Get missing localizations**: Returns a list of languages that haven't been localized yet for the specified content. If you are using Strapi v4 it's necessary to specify the `Strapi version` input as `v4` to ensure compatibility and install the [`graphql` plugin](https://docs-v4.strapi.io/dev-docs/plugins/graphql) in your Strapi instance. The `Strapi version` input is optional and defaults to `v5`.
> Note, if you are using Strapi v4 you must provide sinfular content type, e.g., `article` instead of `articles`. This is required for compatibility with the Strapi graphql plugin.
- **Get text field value**: Returns a text field value from a content by ID.
- **Upload content**: Uploads an HTML file to localize content for a specific language. This action has an optional `Strapi version` input to specify which version of the content to update (default is the v5). If you are using Strapi v4, you can set the `Strapi version` input to `v4` to ensure compatibility.
- **Delete content**: Deletes content by ID.

## Events

- **On content created or updated**: Polling event that periodically checks for new or updated content. Returns a list of content items if new or updated content is found.
- **On content published**: Polling event that periodically checks for newly published content. Returns a list of content items if newly published content is found.

## Example

Here's a simple example of how to use the Strapi app in a Blackbird workflow:

![example](docs/images/example.png)

This bird is triggered when a content is published in Strapi. It then downloads the content, translates it using a `DeepL` app, and finally uploads the translated content back to Strapi.

## Known Limitations

There is no way to retrieve a list of content types from the API, so it's not possible to provide a dropdown list for the **Content type ID** input. Unfortunately, it's the client’s responsibility to enter the content types manually as plain text.

You can get the content type ID from the Strapi UI. Typically, multiple content types will look like: `'users'`, `'articles'`, `'pages'`, and so on.

## Feedback

Do you want to use this app or do you have feedback on our implementation? Reach out to us using the [established channels](https://www.blackbird.io/) or create an issue.

<!-- end docs -->
