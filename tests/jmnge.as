load 1 a
load 2 b
jmnge 2 1 end # if a <= b -> start
dec 1
dec 1
end: dec 1
halt
a .fill 4
b .fill 2

