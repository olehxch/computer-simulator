hello xor 0 1 1		a -> reg1
sad	div 63 63 1 		b -> reg2
	mov 12 2
	JMNGE 1 2 sad
	dec 1
	JMAE 1 2 sad
	halt			end program
knhjgf .fill 2
b .fill 1
des .fill 5643213
