
# SaturnService Project Documentation

## Overview

SaturnService is a web communication solution designed to fetch and process NFL team statistics from the 2020 season. This service utilizes a REST API to retrieve JSON formatted data for each team, enabling detailed analysis and presentation of team performances. A key feature of SaturnService is the integration of a message queue system, specifically Microsoft Message Queuing (MSMQ), to regulate requests to the server and implement remote procedure call (RPC) patterns for asynchronous data processing.

## Features

- **API Communication**: SaturnService communicates with an external API to fetch NFL team statistics, offering insights into each team's performance throughout the 2020 season.
  
- **Message Queue Implementation**: Utilizes MSMQ to manage and regulate requests, ensuring efficient processing and retrieval of data. This setup helps in maintaining the service's responsiveness and scalability.

- **Season Data Analysis**: Processes data for all NFL teams (1-32) from the 2020 season, including team names, team numbers, and season records based on game scores.

- **Flexible and Robust Design**: Designed with object-oriented principles, ensuring the system is robust against unexpected inputs and flexible for future expansions or modifications.

## Functional Requirements

1. **Server Communication**: Capable of interacting with external servers to fetch required data.
2. **JSON Data Presentation**: Supports presenting JSON data in a user-friendly format, either to the console or a file, including team names, numbers, and season records.
3. **Data Storage**: Efficiently stores the JSON response data for further processing or analysis.

## Non-Functional Requirements

1. **Implementation Language**: Developed in C#, leveraging the strengths of object-oriented programming (OOP) for a clean and maintainable codebase.
2. **System Robustness**: Designed to handle unexpected inputs gracefully, ensuring the system's reliability.
3. **Clean and Flexible Design**: Utilizes object-oriented design (OOD) principles to achieve a clean, maintainable, and flexible system architecture.
4. **Maintainability and Reusability**: Emphasizes modular and cohesive units for ease of maintenance and potential reusability in future projects.

## Design and Implementation

### Message Queue Model

The message queue, implemented using MSMQ, acts as a middleware layer that controls the flow of requests to the server. This setup not only helps in load balancing but also in decoupling the data fetching logic from the data processing components, allowing for asynchronous processing of team data. The queue listens for incoming tasks (team data fetch requests) and processes them sequentially, ensuring that each request is handled efficiently and without overwhelming the server.

### System Architecture

The system architecture is built around several key components:

- **SportsService**: Interfaces with the external API to fetch team statistics.
- **QueueManager**: Manages the message queue, ensuring tasks are enqueued and processed asynchronously.
- **TeamManager**: Processes fetched team data, analyzing and storing team performance statistics.
- **TeamRequest and TeamStat Models**: Define the data structure for requests and the format for processed statistics.

### Class and System Models

The class model, detailed using UML diagrams, outlines the relationships and dependencies between the various components of SaturnService. Similarly, the system model illustrates the flow of data from the external API through the message queue to the final processing and presentation stages.
## UML Diagram
![UML Diagram](./SaturnServiceUML.png)
