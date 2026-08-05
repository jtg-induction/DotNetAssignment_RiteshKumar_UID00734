# Restaurant Management System

A backend API system for managing restaurants, users, menus, orders, and role-based access control.

## Overview

Restaurant Management System is a Web API application built using ASP.NET Framework 4.8. It provides APIs for managing restaurant operations with authentication, authorization, and database management using Entity Framework 6.

The system supports different user roles such as:
- Customers
- Restaurant Owners
- Super Admins

## Tech Stack

### Backend
- ASP.NET Web API (.NET Framework 4.8)
- C#
- Entity Framework 6
- SQL Server

### Database
- SQL Server

### Tools
- Visual Studio
- SQL Server Management Studio
- Git

## Features

### Authentication & Authorization
- User registration
- User login
- Role-based access control
- Secure API authorization

### Restaurant Management
- Create and manage restaurants
- Manage restaurant details
- Assign restaurant owners

### Menu Management
- Add menu items
- Update menu items
- Delete menu items
- View restaurant menus

### Order Management
- Create and manage orders
- Track order status

### Admin Management
- Manage users
- Manage restaurants
- Generate reports

## Database Setup

### Prerequisites

Install:

- Visual Studio
- SQL Server
- SQL Server Management Studio

### Configure Connection String

Update the connection string in `Web.config`:

```xml
<connectionStrings>
  <add name="RestaurantDb"
       connectionString="Data Source=localhost;Initial Catalog=RestaurantManagement;Integrated Security=True;"
       providerName="System.Data.SqlClient" />
</connectionStrings>