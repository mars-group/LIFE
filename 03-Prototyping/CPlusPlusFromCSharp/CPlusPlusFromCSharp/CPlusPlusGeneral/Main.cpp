#include <iostream>
#include <string>
#include "Person.h"

using namespace std;

int main(){
	Person *p = new Person;

	string in;
	cin >> in;
	delete p;
	return 0;
}