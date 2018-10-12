# GOTO

This' a program written in GOTO, would you guess its goal?

```
[A] X = X - 1
Y = Y + 1
IF X != 0 GOTO A
```

Exactly! It "copies" the input value in X right into Y, that simple.

## Language definition

### Vars

Input: `X` (or `X1`), `X2`, `X3`, ..., `X8`; provided at startup

Output: `Y`, by default set to 0

Aux.: `Z` (or `Z1`), `Z2`, `Z3`, ..., `Z8`

### Labels

`A1` (or `A`), `B1` (or `B`), `C1` (or `C`), `D1` (or `D`), `E1` (or `E`), `A2`, `B2`, `C2`, `D2`, `E2`, ...

`E` (or `E1`) is a special one, as it's used for exiting.

### Instructions

For each var *V* and label *L*:

- Increment: `V = V + 1`
- Decrement: `V = V - 1`
- Conditional: `IF V != 0 GOTO L`

- Skip: `V = V`
- `[K] Instruction`
  - being *K* a label different from `E`, and
  - *Instruction* any of the above

### Programs

Any finite succession of instructions whose last one isn't `Y = Y`.