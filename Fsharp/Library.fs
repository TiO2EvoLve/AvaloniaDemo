namespace MyFunctionalLib

module Math =

    let add x y =
        x + y

    let square x =
        x * x
        
    let factorial n =
        if n <= 0 then invalidArg "n" "n must be non-negative"
        [1..n] |> List.reduce (*)
