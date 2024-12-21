# UghLang
Written in C# interpreted programing language created for educational purpose and to show the power of simplicity.

# Building
Make sure you have .net 9 installed
```
git clone https://github.com/BiznesBear/UghLang
cd UghLang
dotnet build
```

# Running script
```
ughlang master.ugh
```
### Console arguments
- `--debug` (prints debug tree)
- `--version`
- `--info`
- `--help`

# Mini documentation
This section is dedicated to the basic functions of the ugh language.

## How brackets work in UghLang
() creates new expression which returns somthing.
Wrong:
```
print "Hello " + "world!"; # This won't work, becouse ugh doesn't know how to read it #
```
Correct:
```
print("Hello " + "world!"); # print keyword prints one single value, not a list of them # 
```
Remeber that empty expression (not arguments list) throws you an exception.

## Keywords list
- print
- input
- free
- true
- false
- fun
- break
- return 
- if
- else
- elif
- count
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
# TIP: You can replace name with string "all" to release all names. #

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

### Count
```ugh
myVar = 0;
count(10){
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
### Fun, return, call
```ugh
fun hello(){
	return("Hello");
}

hello();
print(hello + " world!"); # Get value from last hello execution #

# You can use call to assign function value to variables #

myVar = call hello();
print(myVar + " world!");

# The fastest way to do all above #
print(call hello(); + " world!"); # If you want to add any operations you need to add semicolin after calling function #
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
# Examples
## Fibonacci
```ugh
# Fibonacci sequence example #

n = 10; # Lenght #
a = 0; 
b = 1;  

count(n){ 
    c = (a + b); 
    print(c);  
    a = b;     
    b = c;     
}
```