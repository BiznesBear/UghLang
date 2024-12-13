# UghLang
Created in C# for educational purpose interpeted programing language.
The side-purpose of this project is to show the real power of simplicity.

# Mini documentation
This section is dedicated to the basic functions of the ugh language.

## Keywords list
- print
- input
- free
- true
- false
- fun
- break
- if
- else
- repeat

## Variables
### Declaring variables
```ugh
myVar = 5;
print("myVar equals " + myVar);
```

### Releasing resources
```ugh
# TIP: You can replace variable name with string "all" to release all variables. #

myVar = 5;
free myVar;
```

### Math operations
```ugh
myVar = 5; # Set #
myVar + 5; myVar += 5;# Add #
myVar - 5; myVar -= 5;# Subtract #
myVar * 5; myVar *= 5;# Multiply #
myVar / 5; myVar /= 5;# Divide #
myVar ** 2; # Power #
myVar // 2; # Square root #
```

## Statements and loops

### If and else
```ugh
if(5 > 3){
	print("5 is higher than 3");
}
else {
	print("5 is lower than 3");
}
```

### Repeat
```ugh
myVar = 0;
repeat(10){
	myVar + 1;
	print(myVar);
}
```