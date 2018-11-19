using System;
using System.Collections;
using System.Collections.Generic;

namespace Program {

public enum Op {
	ADD, SUB, MUL, DIV, EQU, NEQU, LSS, GTR, LEQ, GEQ, NEG,
	LOAD, LOADG, STO, STOG, CONST, AND, OR, UNION, CROS, RES, DIFF,
	CALL, RET, ENTER, LEAVE, JMP, FJMP, READ, WRITE, ERR
}

	public class Obj {  // объект, описывающий объявленное имя
	public string name;		
	public int type;		
	public Obj	next;		// to next object in same scope
	public int kind;        // var, proc, scope
	public Object value;	
	public int adr;			// address in memory or start of proc
	public int level;		// nesting level; 0=global, 1=local
	public Obj locals;		// scopes: to locally declared objects
	public int nextAdr;		// scopes: next free address in this scope
}
	public class SymbolTable {

	const int // types
		undef_ = 0, int_ = 1, bool_ = 2, byte_ = 3, double_ = 5, string_=6, label_=7, 
	  SetOfint = 8, SetOfbool = 9, SetOfbyte = 10, SetOfreal = 11, SetOfdouble = 12, SetOfstring = 13;

	const int // object kinds
		var_ = 0, proc_ = 1, scope = 2, const_ = 3;

	public int operSum = 0; //Общее количество выполненных операторов

	public int curLevel;	// Уровень вложенности of current scope
	public Obj undefObj;	// объект-узел для ошибочных символов
	public Obj topScope;    // верхняя область процедуры
	public Hashtable labels;
	Parser parser;
	
	public SymbolTable(Parser parser) {
		this.parser = parser;

		labels = new Hashtable();

		topScope = null;
		curLevel = -1;
		undefObj = new Obj();
		undefObj.name  =  "undef_"; undefObj.type = undef_; undefObj.kind = var_;
		undefObj.adr = 0; undefObj.level = 0; undefObj.next = null;
	}

		public Obj NewObj(string name, int kind, int type) {
			Obj p, last, obj = new Obj();
			obj.name = name; obj.kind = kind; obj.type = type;
			obj.level = curLevel;
			p = topScope.locals; last = null;
			while (p != null) {
				if (p.name == name) parser.SemErr("name declared twice");
				last = p; p = p.next;
			}
			if (last == null) topScope.locals = obj; else last.next = obj;
			if (kind == var_) obj.adr = topScope.nextAdr++;
			return obj;

			
		}

		public void NewLabel(string labelName, Token labelOperator) {
			if (!(labels.Contains(labelName))) labels.Add(labelName,labelOperator);
		}

		public Token gotoRealization(string labelName) {
			Token t=null;
			if (labels.Contains(labelName)) t = (Token) labels[labelName];
			return t;
		}

	public Object Oper(int type1, int type2, Object value1, Object value2, Op op ) {
		Object value=0; int type=0;
		int val1Int=0; byte val1Byte=0; double val1Double=0;
		int val2Int=0; byte val2Byte=0; double val2Double=0;
		string val1String=""; bool val1Bool=false;
		string val2String=""; bool val2Bool=false;
		switch (type1) {
			case int_:  val1Int = (int)value1; break;
			case byte_: val1Byte = (byte)value1; break;
			case double_: val1Double = Convert.ToDouble(value1); break;
			case string_: val1String = Convert.ToString(value1); break;
			case bool_: val1Bool = Convert.ToBoolean(value1); break;
		}
		switch (type2) {
			case int_: val2Int = (int)value2; break;
			case byte_: val2Byte = (byte)value2; break;
			case double_: val2Double = Convert.ToDouble(value2); break;
			case string_: val2String = Convert.ToString(value2); break;
			case bool_: val2Bool = Convert.ToBoolean(value2); break;
			}
		switch (op) {		
		case Op.MUL:
			switch (type1) {
				case int_:
					if (type2 == int_) { value = val1Int * val2Int;}
					else if (type2 == byte_) { value = val1Int * val2Byte;}
					else if (type2 == double_) { value = val1Int * val2Double;}
					break;
				case byte_:
					if (type2 == int_) { value = val1Byte * val2Int; }
					else if (type2 == byte_) { value = val1Byte * val2Byte; }
					else if (type2 == double_) { value = val1Byte * val2Double; }
					break;
				case double_:
					if (type2 == int_) { value = val1Double * val2Int; }
					else if (type2 == byte_) { value = val1Double * val2Byte; }
					else if (type2 == double_) { value = val1Double * val2Double; }
					break;
			}
			break;
		case Op.DIV:
			switch (type1) {
				case int_:
					if (type2 == int_) { value = (double) val1Int / (double)val2Int; }
					else if (type2 == byte_) { value = (double)val1Int / (double)val2Byte; }
					else if (type2 == double_) { value = (double) val1Int / (double)val2Double; }
					break;
				case byte_:
					if (type2 == int_) { value = (double) val1Byte / (double) val2Int; }
					else if (type2 == byte_) { value = (double) val1Byte / (double) val2Byte; }
					else if (type2 == double_) { value = (double) val1Byte / (double) val2Double; }
					break;
				case double_:
					if (type2 == int_) { value = (double) val1Double / (double) val2Int; }
					else if (type2 == byte_) { value = (double) val1Double / (double) val2Byte; }
					else if (type2 == double_) { value = (double) val1Double / (double) val2Double; }
					break;
			}
			break;
		case Op.UNION:
			break;
		case Op.CROS:
			break;
		case Op.RES:
			break;
		case Op.DIFF:
			break;
		case Op.ADD:
			switch (type1) {
				case int_:
					if (type2 == int_) { value = val1Int + val2Int; }
					else if (type2 == byte_) { value = val1Int + val2Byte;}
					else if (type2 == double_) { value = val1Int + val2Double;}
					break;
				case byte_:
					if (type2 == int_) { value = val1Byte + val2Int; }
					else if (type2 == byte_) { value = (byte) (val1Byte + val2Byte); }
					else if (type2 == double_) { value = val1Byte + val2Double;}
					break;
				case double_:
					if (type2 == int_) { value = val1Double + val2Int; }
					else if (type2 == byte_) { value = val1Double + val2Byte; }
					else if (type2 == double_) { value = val1Double + val2Double; }
					break;
			}
			break;
		case Op.SUB:
			switch (type1) {
				case int_:
					if (type2 == int_) { value = val1Int - val2Int; }
					else if (type2 == byte_) { value = val1Int - val2Byte; }
					else if (type2 == double_) { value = val1Int - val2Double; }
					break;
				case byte_:
					if (type2 == int_) { value = val1Byte - val2Int; }
					else if (type2 == byte_) { value = (byte)(val1Byte - val2Byte); }
					else if (type2 == double_) { value = val1Byte - val2Double; }
					break;
				case double_:
					if (type2 == int_) { value = val1Double - val2Int; }
					else if (type2 == byte_) { value = val1Double - val2Byte; }
					else if (type2 == double_) { value = val1Double - val2Double; }
					break;
			}
			break;
		case Op.EQU:
					if ((type1 == int_) && (type2 == int_)) value = (val1Int == val2Int);
					if ((type1 == byte_) && (type2 == byte_)) value = (val1Byte == val2Byte);
					if ((type1 == double_) && (type2 == double_)) value = (val1Double == val2Double);
					if ((type1 == bool_) && (type2 == bool_)) value = (val1Bool == val2Bool);
					if ((type1 == string_) && (type2 == string_)) value = String.Equals(val1String, val2String);
			break;

		case Op.NEQU://!=
					if ((type1 == int_) && (type2 == int_)) value = (val1Int != val2Int);
					if ((type1 == byte_) && (type2 == byte_)) value = (val1Byte != val2Byte);
					if ((type1 == double_) && (type2 == double_)) value = (val1Double != val2Double);
					if ((type1 == bool_) && (type2 == bool_)) value = (val1Bool != val2Bool);
					if ((type1 == string_) && (type2 == string_)) value = !(String.Equals(val1String, val2String));
					break;
		case Op.LSS://<
					if ((type1 == int_) && (type2 == int_)) value = (val1Int < val2Int);
					if ((type1 == byte_) && (type2 == byte_)) value = (val1Byte < val2Byte);
					if ((type1 == double_) && (type2 == double_)) value = (val1Double < val2Double);
					break;
		case Op.GTR://>
					if ((type1 == int_) && (type2 == int_)) value = (val1Int > val2Int);
					if ((type1 == byte_) && (type2 == byte_)) value = (val1Byte > val2Byte);
					if ((type1 == double_) && (type2 == double_)) value = (val1Double > val2Double);
					break;
		case Op.LEQ://<=
					if ((type1 == int_) && (type2 == int_)) value = (val1Int <= val2Int);
					if ((type1 == byte_) && (type2 == byte_)) value = (val1Byte <= val2Byte);
					if ((type1 == double_) && (type2 == double_)) value = (val1Double <= val2Double);
					break;
		case Op.GEQ://>=
					if ((type1 == int_) && (type2 == int_)) value = (val1Int >= val2Int);
					if ((type1 == byte_) && (type2 == byte_)) value = (val1Byte >= val2Byte);
					if ((type1 == double_) && (type2 == double_)) value = (val1Double >= val2Double);
					break;
		case Op.AND:
					if ((type1 == bool_) && (type2 == bool_)) value = (val1Bool && val2Bool);
					break;
		case Op.OR:
					if ((type1 == bool_) && (type2 == bool_)) value = (val1Bool || val2Bool);
					break;
		case Op.ERR:
					Console.WriteLine("Error operation");
					break;
		}
		return value;
	}

	public Obj Find (string name) {
		Obj obj, scope;
		scope = topScope;
		while (scope != null) {  //для всех открытых областей
			obj = scope.locals;
			while (obj != null) {  // для всех объектов в этой области
				if (obj.name == name) return obj;
				obj = obj.next;
			}
			scope = scope.next;
		}
		parser.SemErr(name + " is undeclared");
		return undefObj;
	}

	// открыть новую область действия и сделать ее текущей областью (topScope)
	public void OpenScope() {
		Obj scop = new Obj();
		scop.name = ""; scop.kind = scope;
		scop.locals = null; scop.nextAdr = 0;
		scop.next = topScope; topScope = scop;
		curLevel++;
	}


	// закрыть текущую область действия
	public void CloseScope() {
		topScope = topScope.next; curLevel--;
	}


	} // end SymbolTable

} // end namespace