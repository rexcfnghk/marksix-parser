# marksix-parser [![Build status](https://ci.appveyor.com/api/projects/status/t0965lig3ndxg21i?svg=true)](https://ci.appveyor.com/project/rexcfnghk/marksix-parser)

A [Mark Six](http://bet.hkjc.com/marksix/?lang=en) result parser written in F#

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
Your prize for draw #0 is First
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
Your prize for draw #0 is Fixed prize of HK$640.00
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
Your prize for draw #0 is Fixed prize of HK$640.00
Your prize for draw #1 is Second
```