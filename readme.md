# Discrete Time System Sandbox
A simple project that is capable of emulating the behaviour of discrete-time systems given an initial state, input functions.

## Example: Fibonacci sequence
### System definition
```
A = [0 1;1 1]	(2x2 matrix)
B = [0;0]		(2x1 matrix)
C = [1 1]		(1x2 matrix)
D = [0]			(1x1 matrix)

x0 = [1;0]		(2x1 matrix)
u(k) = any function
```
### System output
```
y(0) = 1
y(1) = 2
y(2) = 3
y(3) = 5
y(4) = 8
y(5) = 13
y(6) = 21
y(7) = 34
...
```