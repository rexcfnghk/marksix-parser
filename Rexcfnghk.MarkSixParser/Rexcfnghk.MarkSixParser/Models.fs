module Models

type ChosenNumber = ChosenNumber of int

type DrawResultElements =
    | DrawnNumber of int
    | ExtraNumber of int