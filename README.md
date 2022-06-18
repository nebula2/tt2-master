# TT2Master

This is the source code for the app TT2Master - at least most of it.
I removed IAP and ad stuff. If you run the app from code, it will most likely not even work right now.

Back in the days, this was my first real project. I learned a lot by writing this application. And if I look at it now I want to rewrite the complete thing because I am ashamed of the code :D

This has been a commercial application, but since I do not have the time to maintain this monster anymore, I decided to take it to the public so that if someone wants to keep on, they are now able to do that (or have at least some more or less useful reference).

Let's see where this leads to :)

## Please acknowledge
Code is disastrous in all possible ways and it may contain code comments and variable names which should not be read from children.

## What are those projects?

- `TT2Master.Shared` is just a library containing stuff that is shared across appliation domain
- `TT2Master`, `TT2Master.Android` and `TT2Naster,iOS` are all part of the Xamarin.Forms app
- `TT2Master.Func` contains Azure functions which the app interacts with (like providing news or assets like GH's csv files). Basically makes CRUD on Azure Blob Storage and Azure Table Storage
- `TT2Master.Administration` is used to have an easier time managing the assets (was only used in debug mode)