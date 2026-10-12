# Shopping List

A smallish shopping list that allows a user to add an item, set a budget, and manage what is/isn't bought.

This is designed to cover off stories 1-8:
1. View a list of items on a shopping list
2. Add items to the shopping list
3. Remove stuff from the shopping list
4. When I’ve bought something from my list I want to be able to cross it off the list
5. Persist the data so I can view the list if I move away from the page
6. I want to be able to reorder items on my list
7. Total up the prices
8. Put a spending limit in place, alert me if I go over the limit


### How to run it

#### You will need

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [Node.js](https://nodejs.org/) 22 or newer

#### Run the app

From the repository root:

```bash
# 1. Build the Vue frontend into the API's wwwroot folder
cd client
npm install
npm run build
cd ..

# 2. Start the API, which also serves the frontend
dotnet run --project src/ShoppingListApp.Api
```

Then open http://localhost:5080.

The SQLite database (shoppinglist.db) is created automatically on first run, with one empty shopping list.

#### Run the tests

```bash
# Backend
dotnet test

# Frontend
cd client
npm test
```


### Tech stack

#### Backend

| Technology | What it is | What I used it for |
|---|---|---|
| .NET 10 / C# | Microsoft's development platform and language | The whole backend |
| ASP.NET Core minimal APIs | A lightweight way to build web APIs, mapping URLs straight to methods | The six `/api/list` endpoints the frontend calls |
| Entity Framework Core 10 | Microsoft's ORM, which maps C# classes to database tables | Saving and loading the list and its items |
| SQLite | A small database stored in a single file | Keeping the list between visits (story 5), with nothing to install |
| Problem Details | A standard JSON format for API errors | Sending clear error messages, such as duplicate items (409) and invalid prices (400) |

#### Frontend

| Technology | What it is | What I used it for |
|---|---|---|
| Vue 3 | A JavaScript framework for building pages from components | The shopping list screen |
| TypeScript | JavaScript with types | Catching mistakes early, and keeping the frontend's data shapes matched to the API's |
| Vite | A development server and build tool | Running the app during development, and building it into the API's `wwwroot` folder |
| Pinia | Vue's official state management library | One shared store that holds the list and talks to the API |
| Bootstrap 5 | A CSS framework | Layout, styling, and the mobile-first design |
| vue-draggable-plus | A Vue wrapper for SortableJS* | Drag-and-drop reordering (story 6) |

*I am familiar with SortableJS and it was nice to find a Vue extension of this.

#### Testing

| Technology | What it is | What I used it for |
|---|---|---|
| xUnit | A .NET testing framework | Testing the validation rules, budget maths and service |
| In-memory SQLite | A SQLite database that only exists while a test runs | Testing the service against a real database, so the unique index and case-insensitive names are tested too |
| Vitest | A test runner built for Vite projects | Testing the frontend's money and budget helpers, the store, and components |
| Vue Test Utils + jsdom | Tools for mounting Vue components in a simulated browser | Testing the add item form and budget bar without opening a browser |

### Use of AI

I used Claude (an AI assistant) while building this, mainly for reviewing my code, explaining new concepts, and writing parts of the Vue frontend. I wrote the backend myself with its guidance and fixes and reviewed everything it produced. Since I was learning something new, it was nice to have a companion to help me learn.

### Why?

Rose, you are a WebForms developer, why on earth did you build this in ASP.Net Core? 

A couple of reasons:
1. I really wanted to push myself and learn something new with this mini project. That would allow me to do some homework and show that I'm happy to get on with learning new tech stacks.
2. Why didn't I just build with PHP then? Well, honestly I thought there would be more crossover between WebForms and Core. There is not. If I did this again I would probably try PHP, since it's the tech stack that you actually use.

Something else you may notice - I built this layer by layer rather than getting one part working and then building on it. Since I was learning so much at once, I thought this may be easier to truly understand each section before moving on. In hindsight, building one feature end to end would have suited me better, because I learn best by seeing the data move. This method actually made me slower, which I've noted for future.

### Security

Some security things I kept in mind:
- Server-side validation, then browser validation (don't trust the browser to be secure)
- Entity Framework has internal systems to prevent against SQL injection
- Standard headers on responses
- Errors returned as Problem Details to not expose information

### Room to grow

Enhanced testing:
- Playwright for the FE
- More comprehensive unit testing
    - This was my first time with xUnit and I was surprised how nice and easy it was to setup
- Accessibility standards check 
    - Maybe https://wave.webaim.org/ ?

Other notes:
- Personal design for the app
    - Since I was learning so much for this project I left it in the capable hands of Bootstrap components, but in future I'd like to design my own FE
- Story 10: Security and logins
- Story 9: Email sharing
- EF Migrations


### Time logging

| Date | Action | Time Taken |
| --- | --- | --- |
| 07/10 | Setting up the .NET base | 20 mins |
| 07/10 | Setting up Entities | 10 mins |
| 07/10 | Configuration files | 30 mins |
| 10/10 | Rules and Tests | 60 mins |
| 10/10 | Services, DTOs and results | 20 mins |
| 11/10 | Tests! (and fixes) | 60 mins | 
| 11/10 | Fixes (didn't build my API correctly) | 10 mins |
| 11/10 | API Endpoints and testing | 60 mins |
| 12/10 | Vue setup and basic FE | 90 mins |