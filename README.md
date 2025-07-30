# DrugPreventionAPI

## Overview

DrugPreventionAPI is a C# backend application designed to support drug prevention initiatives. It provides APIs for managing surveys, quizzes, educational materials, activities, blog posts, and inquiries related to drug prevention.

## Main Features

- **Survey Management**: Create, update, and manage surveys, including survey questions, submissions, and substances.
- **Quiz System**: Provides functionality for quizzes, including question banks and options.
- **Course Materials**: Manage course materials and resources for drug prevention education.
- **Activity Tracking**: Track participation in communication activities and other events.
- **Blog & Comments**: Manage blog posts, comments, and tags for community engagement.
- **Consultation & Scheduling**: Handle appointment requests and consultant schedules.
- **Certificates**: Issue certificates for completed courses or participation.
- **Inquiry System**: Assign and manage user inquiries and responses.

## Repository Structure

- `DrugPreventionAPI/Repositories/`: Contains repository classes for data access and business logic.
- `DrugPreventionAPI/Models/`: Data models for surveys, quizzes, blogs, etc.
- `DrugPreventionAPI/DTO/`: Data transfer objects for API communication.
- `DrugPreventionAPI/Data/`: Database context and configuration.
- `DrugPreventionAPI/Program.cs`: Application entry point and service registration.

## Technologies

- **Language**: C#
- **Framework**: .NET Core
- **Database**: Entity Framework Core

## Getting Started

Clone the repository and configure your database connection in `DrugPreventionAPI/Data/`. Build and run the API using .NET Core CLI or Visual Studio.

## Contributing

Issues and pull requests are welcome to improve features, fix bugs, or enhance documentation.
