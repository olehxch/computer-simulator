hello xor 0 1 1		a -> reg1
	div 63 63 a	b -> reg2
	mov 12 1
	dec 1 1
	JMAE 1 2 sad
	halt			end program
a .fill 2
b .fill 1
des .fill 5643213
