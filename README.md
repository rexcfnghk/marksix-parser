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
The draw results are [DrawNumber 1; DrawNumber 2; DrawNumber 3; DrawNumber 4; DrawNumber 5; DrawNumber 6; ExtraNumber 7]
Enter user's draw
1
2
3
4
5
6
User's draw is [1; 2; 3; 4; 5; 6]
Your prize is First
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
The draw results are [DrawNumber 1; DrawNumber 2; DrawNumber 3; DrawNumber 4; DrawNumber 5; DrawNumber 6; ExtraNumber 7]
Enter user's draw
1
2
3
4
48
49
User's draw is [1; 2; 3; 4; 48; 49]
Your prize is Fixed prize of HK$640.00
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
The draw results are [DrawNumber 1; DrawNumber 2; DrawNumber 3; DrawNumber 4; DrawNumber 5; DrawNumber 6; ExtraNumber 7]
Enter user's draw
1
2
3
4
48
49
User's draw is [1; 2; 3; 4; 48; 49]
Your prize is Fixed prize of HK$640.00
```