# UghLang
Created in C# for educational purpose interpeted programing language.
The side-purpose of this project is to show the power of simplicity.

# Mini documentation
This section is dedicated to the basic functions of the ugh language.

## Hello world!
```ugh
print("Hello, world!");
```

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
