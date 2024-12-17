# UghLang
Written in C# interpreted programing language created for educational purpose and to show the power of simplicity.

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
- return (not implemented yet)
- if
- else
- elif
- for
- while
- insert
- local


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


### If and else and elif
```ugh
if(5 > 3){
	print("5 is higher than 3");
}
elif(5 == 3){
	print("5 equals 3");
}
else {
	print("5 is lower than 3");
}
```

### For
```ugh
myVar = 0;
for(10){
	myVar + 1;
	print(myVar);
}
```
### While
```ugh
myVar = 0;
while(myVar < 10){
	myVar + 1;
	print(myVar);
}
```
### Insert
```ugh
insert "std"; # You can put any name of ugh file here (or directory which contains source.ugh)#
helloworld(); 
```

### Local
```ugh
local fun foo(){ # Nodes marked as local won't load when inserted from other file # 
	print("Hello, world!");
} 

local { # Example of nested local #

	for(100){
		print("Hello, world!");
	}
}
```