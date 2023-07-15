using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualBasic;

namespace Json
{
    class Json
    {
        unsafe static void Main(string[] args)
        {
            string path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"./../../../input.json");
            String content = File.ReadAllText(path);
            Stack<node> stack = new Stack<node>();
            List<token> tokens = lexer.lex(content);
            tokenType initToken = tokens.First().tokenType;
            tokens.RemoveAt(0);
            if (initToken == tokenType.openObj) {
                node _c = new node();
                _c.nodeType = nodeType._object;
                stack.Push(_c);
            } else {
                // todo
            }

            while (tokens.Count > 1)
            {
                token cToken = tokens.First();
                tokens.RemoveAt(0);
                if (cToken.tokenType == tokenType.open || cToken.tokenType == tokenType.openObj
                                                       || cToken.tokenType == tokenType.openArray) {
                    node _c = new node();
                    node tObj = stack.Last();
                    stack.Push(_c);
                    if (cToken.tokenType == tokenType.openArray) {
                        _c.nodeType = nodeType._array;
                    } else {
                        _c.nodeType = nodeType._object;
                    }
                    if (cToken.tokenType == tokenType.openObj) {
                        tObj.childrenArray.Add(_c);
                    } else {
                        tObj.children.Add(cToken.value,_c);
                    }
                }
                else if (cToken.tokenType == tokenType.value) {
                    String[] result = cToken.value.Split(":");
                    node tObj = stack.Peek();
                    tObj.values.Add(result[0],result[1]);
                } else if (cToken.tokenType == tokenType.primitive) {
                    node tObj = stack.Peek();
                    tObj.valuesPrimitive.Add(cToken.value);
                } else if (cToken.tokenType == tokenType.closed) {
                    stack.Pop();
                }
                
            }
            Console.WriteLine(stack.First().children["menu"].values["header"]);


        }
    }
}

enum tokenType
{
    openObj,
    open,
    openArray,
    primitive,
    value,
    closed,
};

enum nodeType {
_object,
_array
}
class node {
    public Dictionary<String, node> children = new Dictionary<string, node>();
    public List<node> childrenArray = new List<node>();
    public Dictionary<String, String> values = new Dictionary<string, string>();
    public List<String> valuesPrimitive = new List<String>();
    public nodeType nodeType;
}

class token {
    public token(String _value, tokenType _tokenType) {
        this.value = _value;
        this.tokenType = _tokenType;
    }

    public String value;
    public tokenType tokenType;
}

class lexer
{
    static public List<token> lex(String str)
    {
        List<token> tokens = new List<token>();
        String t = "";
        int j = 0;
        Boolean isString = false;
        var next = () =>
        {
            for (int i = j; i <= str.Length - 1; i++)
            {
                {
                    if (str[(i)] == ' ' && isString)
                    {
                        j = i + 1;
                        return str[i];
                    }

                    if (str[i] == ' ' || str[i] == '\n' || str[i] == '\t' || str[i] == '\r')
                    {
                        if (i == str.Length - 1)
                        {
                            j = i + 1;
                        }

                        continue;
                    }
                    else
                    {
                        j = i + 1;
                        return str[i];
                    }
                }
            }

            return 'x';
        };
        var nextnext = (char val) =>
        {
            for (int i = j; i <= str.Length - 1; i++)
            {
                if (str[i] == ' ' || str[i] == '\n' || str[i] == '\t' || str[i] == '\r')
                {
                    continue;
                }
                else
                {
                    if (str[i] == val)
                    {
                        j = i + 1;
                        return str[i];
                    }
                    else
                    {
                        return 'x';
                    }
                }
            }

            return 'x';
        };
        for (int i = j; i <= str.Length - 1; i++)
        {
            char cChar = next();
            if (cChar == '\"')
            {
                isString = !isString;
                continue;
            }

            if (cChar == '{')
            {
                token _c = new token(t, tokenType.openObj);
                tokens.Add(_c);
                t = "";
            }
            else if (cChar == ':' && nextnext('{') == '{')
            {
                token _c = new token(t, tokenType.open);
                tokens.Add(_c);
                t = "";
            }
            else if (cChar == ':' && nextnext('[') == '[')
            {
                token _c = new token(t, tokenType.openArray);
                tokens.Add(_c);
                t = "";
            }
            else if (cChar == ',' || cChar == '}' || cChar == ']')
            {
                if (t.Length > 0)
                {
                    Boolean isPrimitive = t.Contains(":");
                    if (!isPrimitive)
                    {
                        token _c = new token(t, tokenType.primitive);
                        tokens.Add(_c);
                    }
                    else
                    {
                        token _c = new token(t, tokenType.value);
                        tokens.Add(_c);
                    }

                    t = "";
                }

                if (cChar == '}' || cChar == ']')
                {
                    token _c = new token(t, tokenType.closed);
                    tokens.Add(_c);
                }
            }
            else
            {
                t = t + cChar;
            }

        }

        return tokens;

    }

}