﻿# Name Pairing

### A simple tool for drawing names anonymously from a hat, perfect for Secret Santa, gift exchanges etc.

## Introduction

Name Pairing is a small Blazor WASM application designed for Secret Santa groups but it can be used anywhere you need to have a group "draw names" such as gift exchanges, party games, anything really.

It's designed to be a zero-trust, stateless, client-side app with persistent self-contained links. That's a lot of jargon that basically means:

- You can host it anywhere you like (or use any publicly hosted version)
- It doesn't require any storage, databases, etc
- The "server" doesn't gather any information and doesn't even store the links you generate
- Once you've generated the links (see below) that's it: the links will always work and all the information you need is in the link.

## How does it work?

In short, the organizer adds each participant, then clicks *Generate Links*. Your browser will match each participant to another participant and generate links you can share to each participant. The links do not give away who they have drawn but each participant gets their own unique link that will tell them who they have drawn when they open it.

> This way, assuming they don't open the links, even the organizer won't know who drew who!

The organizer shares each participant's link privately, they click the link, and find out who they've drawn. That's it!

## How do I use it?

If you just want to set up a quick event and don't want to use your own URL, you can access a public version of this app [here](https://agc93.github.io/name-pairing/). 

> Don't worry, there is no "server" so your data is never stored anywhere outside your browser!

You'll get shown the links for your participants that you can copy-paste directly or download them all in a text file. Share the links with your participants and you're good to go!

## Can I run it myself?

**Yes!** It's specifically designed to be self-hostable, and incredibly easy to do so. The final application is purely static so essentially any web host of any kind will do. 

The easiest way to get started is to grab the latest build from the Artifacts section of the [CI builds](https://github.com/agc93/name-pairing/actions/workflows/static.yml). If you want to build it yourself, just clone this repo, make sure you have the right .NET 7 SDK installed then run `dotnet publish -o release -c Release ./src/NamePairing/` and the `release/wwwroot` folder will have a complete ready-to-deploy copy of the site.

### Customization

The advantage of running it yourself is that you can also customize some parts of the application! The published output includes an `appsettings.json` that you can use to customize some of the details on the site. Here's what a full config looks like: 

```json
{
  "Customization": {
    "AppName": "Your app name",
    "AppHeader": "Any header text",
    "GreetingPrefix": "Hello",
    "MatchText": "You have drawn"
  }
}
```
Here's what each of those does:

- `AppName`: shown as the tab/window title in the browser
- `AppHeader`: the name shown in the bar at the top of the application
- `GreetingPrefix`: a prefix shown before a participant's name when they view their link.
- `MatchText`: The text shown to introduce who a participant has drawn.

You can provide all, none, or anything in between and NamePairing will fall back to default values if they're not provided.

> Remember you can also fork this and make whatever customizations you may want as well, just remember to follow the LICENSE!