# ddd-efteling
Repo to learn domain driven development. **Note that this has no relationship with Efteling itself, this is a project where the theme park is used as the domain to work with.**

## Introduction

Using the book "Implementing Domain-Driven Design", I wanted to have a subject to work with. That is this repo! So feel free to look around, or fork or anything if you'd like.

24 May 2020: Still working on setting up the base functionality.

07 Jun 2020: Almost all base functionality is in now, only catering needs to be done, then I want to start with some first features and linking it together.

23 Aug 2020: The basic code is available and works. Still a lot to implement, but first I want to read the book a bit more. I am looking into possible functionalities to implement, but also for instance the design patterns to use.

28 Feb 2021: Long time since updating Readme. I am planning to remove these date-like updates on the project. I have implemented some functions in between when I had time, and will start now improving the code and add new functionality. Main objective is to (of course) implement stands, which I am now working on, and fix a few minor bugs. Also I improve front-end but I am still not satisfied (I am not a really good front-end guy, I can do it, but it's not my cup of tea). Also I have finished the book, so the plan is to use aggregates more, split functions for each container, maybe create some services or value objects to clean the code where possible. Finally, I was thinking of looking for path finding, but that is a very future thing.

15 May 2021: Implemented stands and updated some code. Also improved the Docker setup, so that it now is easier to run locally, by including a Kafka container.

# How to run locally

The project consists of two parts: The front-end and the back-end. So those need to be started separately

## Start and stop back-end

Requirements: Docker and the .NET 5.0 SDK

- In a console, go to the root directory of the project (where `docker-compose.yml` is included)
- Run `docker compose build`
- Run `docker compose up`

To stop, run `docker compose down`

## Start and stop front-end

Requirements: NodeJS

- In a console, go to the folder called `efteling-frontend`
- Run `npm install`
- Run `npm start`

The React application will start now and should automatically open the browser. If not, the link should be `http://localhost:3000/`. It could take a minute when it is starting.

To stop, just press `control+c`
