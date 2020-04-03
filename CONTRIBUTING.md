# Contribute to the VIAcode Products documentation

This document covers the process for contributing to the configuration guider for open-source VIAcode products.Typo corrections and new articles are welcome contributions.

## How to make a simple correction or suggestion

Documentation are stored in the repository as Markdown files. Simple changes to the content of a Markdown file are made in the browser by selecting the **Edit** link in the upper-right corner of the browser window. (In a narrow browser window, expand the **options** bar to see the **Edit** link.) Follow the directions to create a pull request (PR). We will review the PR and accept it or suggest changes.

## How to make a more complex submission

You need a basic understanding of [Git and GitHub.com](https://guides.github.com/activities/hello-world/).

* Open an [issue](https://github.com/VIAcode/VIAcode-Incident-Management-System-for-Azure/issues/new) describing what you want to do, such as changing an existing document or creating a new one. We often request an outline for a new topic suggestion. Wait for approval from the team before you invest much time.
* Fork the [VIAcode/VIAcode-Incident-Management-System-for-Azure](https://github.com/VIAcode/VIAcode-Incident-Management-System-for-Azure/) repo and create a branch for your changes.
* Submit a PR to master with your changes.
* Respond to PR feedback.

## Markdown syntax

Articles are written in [GitHub-flavored Markdown (GFM)](https://guides.github.com/features/mastering-markdown/).

## Folder structure conventions

For each Markdown file, a folder for images and a folder for sample code may exist. If the article is [fundamentals/configuration/index.md](https://github.com/dotnet/AspNetCore.Docs/blob/master/aspnetcore/fundamentals/configuration/index.md), the images are in [fundamentals/configuration/index/\_static](https://github.com/dotnet/AspNetCore.Docs/tree/master/aspnetcore/fundamentals/configuration/index/_static) and the sample app project files are in [fundamentals/configuration/index/sample](https://github.com/dotnet/AspNetCore.Docs/tree/master/aspnetcore/fundamentals/configuration/index/sample). An image in the *fundamentals/configuration/index.md* file is rendered by the following Markdown:

```md
![description of image for alt attribute](configuration/index/_static/imagename.png)
```

All images should have [alternative (alt) text](https://wikipedia.org/wiki/Alt_attribute). For advice on specifying alt text, see online resources, such as [WebAIM: Alternative Text](https://webaim.org/techniques/alttext/).

Use lowercase for Markdown file names and image file names.

## Internal links

## Images and screenshots

Don't include images with articles, except:

* In basic onboarding (beginner) tutorials.
* When an image is needed for clarity.

These restrictions reduce the size of the repository.

As an optional step, ensure that any images and screenshots used in the documentation are compressed, which helps with file size and page load performance. A few popular tools include TinyPNG (using the [TinyPNG website](https://tinypng.com/) or the [TinyPNG API](https://tinypng.com/developers)) or the [Image Optimizer](https://marketplace.visualstudio.com/items?itemName=MadsKristensen.ImageOptimizer) Visual Studio extension. 

## Code snippets

Articles frequently contain code snippets to illustrate points. DFM allows you to copy code into the Markdown file or refer to a separate code file. We prefer to use separate code files whenever possible to minimize the chance of errors in the code. The code files are stored in the repo using the folder structure described earlier for sample projects. 

The following examples illustrate [DFM code snippet syntax](https://dotnet.github.io/docfx/spec/docfx_flavored_markdown.html#code-snippet) for use in a *configuration/index.md* file.

To render an entire code file as a snippet:

```md
[!code-csharp[](configuration/index/sample/Program.cs)]
```

To render a portion of a file as a snippet by using line numbers:

```md
[!code-csharp[](configuration/index/sample/Program.cs?range=1-10,20,30,40-50]
[!code-html[](configuration/index/sample/Views/Home/Index.cshtml?range=1-10,20,30,40-50]
```

For C# snippets, reference a [C# region](https://docs.microsoft.com/dotnet/csharp/language-reference/preprocessor-directives/preprocessor-region). Whenever possible, use regions rather than line numbers because line numbers in a code file tend to change and become out of sync with line number references in Markdown. C# regions can be nested. If referencing the outer region, the inner `#region` and `#endregion` directives aren't rendered in a snippet. 

To render a C# region named "snippet_Example":

```md
[!code-csharp[](configuration/index/sample/Program.cs?name=snippet_Example)]
```

To highlight selected lines in a rendered snippet (usually renders as yellow background color):

```md
[!code-csharp[](configuration/index/sample/Program.cs?name=snippet_Example&highlight=1-3,10,20-25)]
[!code-csharp[](configuration/index/sample/Program.cs?range=10-20&highlight=1-3]
[!code-html[](configuration/index/sample/Views/Home/Index.cshtml?range=10-20&highlight=1-3]
[!code-javascript[](configuration/index/sample/UsingOptionsSample.csproj?range=10-20&highlight=1-3]
```

## Voice and tone

Our goal is to write documentation that is easily understandable by the widest possible audience. To that end, we established guidelines for writing style that we ask our contributors to follow. For more information, see [Voice and tone guidelines](https://docs.microsoft.com/en-us/contribute/dotnet/dotnet-voice-tone).

## Microsoft Writing Style Guide

The [Microsoft Writing Style Guide](https://docs.microsoft.com/style-guide/welcome/) provides writing style and terminology guidance for all forms of technology communication.
