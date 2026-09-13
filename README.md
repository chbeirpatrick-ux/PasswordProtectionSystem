# Password Protection System (C#)

So here's the thing about passwords.

Every time you sign up for something, somewhere, a computer is deciding
how to remember what you just typed. That decision matters more than you'd
think — because databases leak. A lot. And when they do, the way your
password was stored decides whether you're mildly inconvenienced or
completely screwed.

This project is a tiny demo of that, written in **C#** and built in
**Visual Studio 2022**. You register a user, and my little console app
saves your password three different ways at the same time. Two of those
ways are terrible. One of them is how real systems do it.

You can see the difference with your own eyes in about ten seconds.

---

## What actually happens when you hit "Register"

You type a username. You type a password. That's your job done.

Behind the scenes, my C# program:

1. Invents a random-looking user ID for you (a UUID via `Guid.NewGuid()`)
2. Writes your password into **Table 1** exactly as you typed it
3. Hashes your password with SHA-256 and drops the result into **Table 2**
4. Generates a unique salt, mixes it with your password, hashes that, and
   sticks it in **Table 3**

Then it proudly prints all three results on screen. No database, no cloud,
no setup. Just three `List<T>` collections in memory and a point to make.

There's also a login option — that was the bonus part — that checks your
password against all three tables. Spoiler: all three methods "work."
But only one of them is safe.

---

## The three tables, ranked from worst to best

### 🚨 Table 1 — Plain text

This is exactly what it sounds like. Your password, sitting in a table,
readable by anyone who opens the file.

If this database leaks, the attacker doesn't need to "hack" anything.
They just read. That's the whole attack.

RockYou, 2009 — 32 million passwords, all in plain text, all immediately
public. If any of those users used the same password on their Gmail or
their bank, those accounts fell too. That's called credential stuffing,
and it's still one of the most common ways people get hacked today.

Saving plain text passwords should honestly be illegal. In some places,
it is.

### ⚠️ Table 2 — SHA-256 hash, no salt

Now we've scrambled it. `P@ssw0rd123` becomes a 64-character hex string
that means nothing to a human. Progress!

Except. Two things bite you here.

**Problem #1: Rainbow tables.** Someone, somewhere, already took a huge
list of common passwords and hashed every single one. That file is called
a rainbow table. It's basically a phone book, but instead of
`name → phone number` it's `password → hash`. Millions and millions of
entries. Freely downloadable.

So if your password is anywhere near common, the attacker doesn't need
to crack anything. They just look up your hash. Instant. No effort.

**Problem #2: Matching hashes.** Hashing is deterministic. Same input,
same output. So if two users pick the same password — say, `qwerty123` —
their stored hashes are **identical**. The attacker cracks one, cracks
both. And they can see at a glance that these two users share a password,
which is already information they shouldn't have.

This is how the LinkedIn breach worked in 2012. Unsalted SHA-1 hashes.
Around 90% of them got cracked within days.

### ✅ Table 3 — SHA-256 hash, with salt

Now we're talking.

A salt is a unique random value created for each user. Before hashing,
you mix it in with the password:

    stored_hash = SHA256(salt + password)

And then you save the salt right next to the hash. Yes, right there,
in plain sight. Nobody cares that the attacker can see the salt — that's
the entire design. The salt doesn't need to be secret. It needs to be
**unique**.

Now look what happens:

**Rainbow tables stop working.** The precomputed table has an entry for
`SHA256("qwerty123")`. It does **not** have an entry for
`SHA256("7d3a9b2c1e4f5a6b8c9d0e1f2a3b4c5d" + "qwerty123")`. Nobody's
building tables for every possible salt. There are more possible salts
than there are atoms in the observable universe. Good luck.

**Identical passwords look different.** Alice picks `P@ssw0rd123`.
Bob picks `P@ssw0rd123`. Their Table 3 hashes have nothing to do with
each other. The attacker can't tell they match, and has to attack each
one separately.

That's the whole point. Salt doesn't make a weak password strong. But it
turns "crack 10 million users at once" into "crack 10 million users, one
by one, individually, please enjoy."

Which nobody has time for.

---

## A quick note about the salt in this project

Real-world salts are **random** — generated fresh per user by a
cryptographically secure random number generator.

The assignment specifically asks for a **deterministic** salt instead,
so we can reproduce it and see the mechanism clearly. Mine looks like:

    salt = SHA256(username + userId + timestamp)

That's why Table 3 stores the timestamp — we need it to rebuild the same
salt when you log back in.

I want to be honest about this: it's a teaching version. In any real
system, you'd use something like `RandomNumberGenerator.GetBytes(16)`.
The assignment says not to use random number generators unless they come
from a crypto library, so I stuck with the reproducible approach it asks
for. Nothing in this repo is production code.

---

## Tech stack

Everything in this project is written in **C#** on **.NET 8**, built and
run in **Visual Studio 2022**. There's no JavaScript, no Python, no
framework — just a plain C# console app.

- **Language:** C# (that's it, the whole project)
- **Runtime:** .NET 8.0 (works on 6.0 too if you're on an older SDK)
- **IDE:** Visual Studio 2022, with the ".NET desktop development" workload
- **Hashing:** `System.Security.Cryptography.SHA256` — the built-in C#
  library. I didn't write any hashing code myself, and you shouldn't
  either.
- **UUIDs:** `System.Guid.NewGuid()` — C#'s built-in UUID generator
- **Storage:** plain `List<T>` collections in memory. No database, no
  Entity Framework, no SQL. The assignment doesn't need persistence,
  so I didn't add any.
- **UI:** the C# console. `Console.WriteLine`, `Console.ReadKey`,
  the whole retro vibe.

---

## Project layout

    PasswordProtectionSystem/
    ├── Program.cs                    # Console menu, main flow
    ├── Models/
    │   └── UserRecord.cs             # The three table classes
    ├── Services/
    │   ├── HashService.cs            # SHA-256 wrapper
    │   ├── SaltService.cs            # Deterministic salt
    │   └── UserService.cs            # Register + login logic
    ├── Data/
    │   └── InMemoryDatabase.cs       # Three lists acting as tables
    ├── .gitignore
    ├── PasswordProtectionSystem.sln  # Visual Studio solution file
    └── README.md

---

## How to run it

You need the .NET SDK (6.0+) and **Visual Studio 2022** with the
".NET desktop development" workload installed. That's it. No database
setup, no config files, no secrets to paste anywhere.

    git clone https://github.com/YourUsername/PasswordProtectionSystem.git

Open `PasswordProtectionSystem.sln` in Visual Studio. Hit **F5**. You're in.

Menu options:

    [1] Register a new user
    [2] Login (bonus)
    [3] Exit

---

## What a run looks like

    ========== REGISTRATION SUCCESSFUL ==========
    User ID   : 3f2504e0-4f89-11d3-9a0c-0305e82c3301
    Username  : alice
    Timestamp : 2025-01-15T14:32:11.1234567Z

    --- Table 1: PLAIN TEXT (INSECURE) ---
    Password  : P@ssw0rd123

    --- Table 2: SHA-256 HASH (NO SALT) ---
    Hash      : 9f8a7c6e5d4b3a2918273645f0e1d2c3b4a59687...

    --- Table 3: SALTED SHA-256 (SECURE) ---
    Salt      : 7d3a9b2c1e4f5a6b8c9d0e1f2a3b4c5d6e7f8a9b...
    SaltedHash: 1a2b3c4d5e6f7a8b9c0d1e2f3a4b5c6d7e8f9a0b...

Now here's the fun part. Register a **second** user with the exact same
password. Watch Table 2. The hash will be **identical** to alice's.
Then watch Table 3. Completely, totally, unrecognizably different.

That contrast is the whole assignment in a single screenshot. If you only
look at one thing in this repo, look at that.

---

## Why C# for this project

A few reasons I went with C# instead of Python or JavaScript:

- The assignment lets you pick any language, and C# has a rock-solid
  built-in crypto library (`System.Security.Cryptography`) that's been
  audited for years.
- Visual Studio gives you a proper project structure (`.sln` + `.csproj`)
  which makes it easy to submit and grade.
- String handling, hex encoding, and file structure in C# are all clean
  and predictable — no surprises when you convert byte arrays to hex.
- It's the language I'm most comfortable writing security-adjacent code in.

---

## Rules from the assignment (all followed)

- ✅ Used an existing hashing library (`System.Security.Cryptography.SHA256`)
- ✅ Did not write my own hash function
- ✅ Salt is unique per user
- ✅ Salt is deterministic and reproducible
- ✅ No random number generators used
- ✅ Never reuse the same salt
- ✅ No weak or reversible hashing
- ✅ No encryption keys stored in code





