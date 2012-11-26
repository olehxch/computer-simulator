hello xor 0 1 1		a -> reg1
	div 63 61 1 a	b -> reg2
	mov 12 12		as
	dec 1
	JMAE 1 2 hello
	halt			end program
a .fill 2
b .fill 1
des .fill 5643213
