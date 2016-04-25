# marksix-parser [![Build status](https://ci.appveyor.com/api/projects/status/t0965lig3ndxg21i?svg=true)](https://ci.appveyor.com/project/rexcfnghk/marksix-parser) [![Coverage Status](https://coveralls.io/repos/github/rexcfnghk/marksix-parser/badge.svg?branch=feature%2Fadd-code-coverage)](https://coveralls.io/github/rexcfnghk/marksix-parser?branch=feature%2Fadd-code-coverage)

A [Mark Six](http://bet.hkjc.com/marksix/?lang=en) result parser written in F#

Installation
---
This project uses [Paket](https://fsprojects.github.io/Paket/) as the package manager and [FAKE](http://fsharp.github.io/FAKE/) as the build automation tool.

Run `build.sh` or `build.cmd` to:
  - Install dependencies using Paket
  - Clean `./build/`, `./tests/`, and `./release/` directories
  - Build the app (to `./build/`)
  - Build the tests (to `./tests/`)
  - Run xUnit tests
  - Run OpenCover and save results (to `./tests/results.xml`)
  - Zip the artifact (to `./release/marksix-parser.zip`)
  
If you simply want to install dependencies and start developing, run `.paket/paket.bootstrapper.exe` and `paket restore`.

Sample output
---
```
Enter draw results
1
2
3
4
5
6
7
The draw results are DrawResults
  (DrawnNumber 1,DrawnNumber 2,DrawnNumber 3,DrawnNumber 4,DrawnNumber 5,
   DrawnNumber 6,ExtraNumber 7)
   
Enter user's #1 draw
1
2
3
4
5
6
Continue entering user's draw #2 [YyNn]?
n
User's draw #1: UsersDraw (1,2,3,4,5,6)
You entered 1 user's draw(s)
Your prize for draw #1 is First
```

```
Enter draw results
1
2
3
4
5
6
7
The draw results are DrawResults
  (DrawnNumber 1,DrawnNumber 2,DrawnNumber 3,DrawnNumber 4,DrawnNumber 5,
   DrawnNumber 6,ExtraNumber 7)
   
Enter user's #1 draw
1
2
3
4
48
49
Continue entering user's draw #2 [YyNn]?
n
User's draw #1: UsersDraw (1,2,3,4,48,49)
You entered 1 user's draw(s)
Your prize for draw #1 is Fixed prize of HK$640.00
```

```
Enter draw results
1
1
ErrorMessage "Adding duplicate elements"
2
3
4
5
6
50
ErrorMessage "Input out of range"
7
The draw results are DrawResults
  (DrawnNumber 1,DrawnNumber 2,DrawnNumber 3,DrawnNumber 4,DrawnNumber 5,
   DrawnNumber 6,ExtraNumber 7)
   
Enter user's #1 draw
1
2
3
4
48
49
Continue entering user's draw #2 [YyNn]?
y
Enter user's #2 draw
2
3
4
sdf
ErrorMessage "Input is not an integer"
5
6
7
Continue entering user's draw #3 [YyNn]?
n
User's draw #1: UsersDraw (1,2,3,4,48,49)
User's draw #2: UsersDraw (2,3,4,5,6,7)
You entered 2 user's draw(s)
Your prize for draw #1 is Fixed prize of HK$640.00
Your prize for draw #2 is Second
```
