load 1 a
load 3 c
load 4 inc
load 5 bound
start: add 1 3 1
add 3 4 3
jmnge 3 5 start
save 1 @mem
halt
a .fill 0
b .fill 1
c .fill 1
inc .fill 1
bound .fill 5
mem .fill 0