using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Stanford.NER.Net.Util
{
    public class MetaClass
    {
        public class ClassCreationException : Exception
        {
            public ClassCreationException()
                : base()
            {
            }

            public ClassCreationException(string msg)
                : base(msg)
            {
            }

            public ClassCreationException(Exception cause)
                : base("ClassCreationException", cause)
            {
            }

            public ClassCreationException(string msg, Exception cause)
                : base(msg, cause)
            {
            }
        }

        public sealed class ConstructorNotFoundException : ClassCreationException
        {
            public ConstructorNotFoundException()
                : base()
            {
            }

            public ConstructorNotFoundException(string msg)
                : base(msg)
            {
            }

            public ConstructorNotFoundException(Exception cause)
                : base(cause)
            {
            }

            public ConstructorNotFoundException(string msg, Exception cause)
                : base(msg, cause)
            {
            }
        }

        public sealed class ClassFactory<T>
        {
            private Type[] classParams;
            private Type cl;
            private ConstructorInfo constructor;
            private static bool SamePrimitive(Type a, Type b)
            {
                if (!a.IsPrimitive && !b.IsPrimitive)
                    return false;

                if (a.IsPrimitive)
                {
                    try
                    {
                        Type type = (Type)b.GetField(@"TYPE").GetValue(null);
                        return type.Equals(a);
                    }
                    catch (Exception e)
                    {
                        return false;
                    }
                }

                if (b.IsPrimitive)
                {
                    try
                    {
                        Type type = (Type)a.GetField(@"TYPE").GetValue(null);
                        return type.Equals(b);
                    }
                    catch (Exception e)
                    {
                        return false;
                    }
                }

                throw new InvalidOperationException(@"Impossible case");
            }

            private static int SuperDistance(Type candidate, Type target)
            {
                if (candidate == null)
                {
                    return int.MinValue;
                }
                else if (candidate.Equals(target))
                {
                    return 0;
                }
                else if (SamePrimitive(candidate, target))
                {
                    return 0;
                }
                else
                {
                    Type directSuper = candidate.BaseType;
                    int superDist = SuperDistance(directSuper, target);
                    if (superDist >= 0)
                        return superDist + 1;
                    Type[] interfaces = candidate.GetInterfaces();
                    int minDist = int.MaxValue;
                    foreach (Type i in interfaces)
                    {
                        superDist = SuperDistance(i, target);
                        if (superDist >= 0)
                        {
                            minDist = Math.Min(minDist, superDist);
                        }
                    }

                    if (minDist != int.MaxValue)
                        return minDist + 1;
                    else
                        return -1;
                }
            }

            private void Construct(string classname, params Type[] params_renamed)
            {
                this.classParams = params_renamed;
                try
                {
                    this.cl = (Type)Type.GetType(classname);
                }
                catch (InvalidCastException e)
                {
                    throw new ClassCreationException(@"Class " + classname + @" could not be cast to the correct type");
                }

                ConstructorInfo[] constructors = cl.GetConstructors(BindingFlags.Instance | BindingFlags.Public);
                ConstructorInfo[] potentials = new ConstructorInfo[constructors.Length];
                Type[][] constructorParams = new Type[constructors.Length][];
                int[] distances = new int[constructors.Length];
                for (int i = 0; i < constructors.Length; i++)
                {
                    constructorParams[i] = constructors[i].GetParameterTypes();
                    if (params_renamed.Length == constructorParams[i].Length)
                    {
                        potentials[i] = constructors[i];
                        distances[i] = 0;
                    }
                    else
                    {
                        potentials[i] = null;
                        distances[i] = -1;
                    }
                }

                for (int paramIndex = 0; paramIndex < params_renamed.Length; paramIndex++)
                {
                    Type target = params_renamed[paramIndex];
                    for (int conIndex = 0; conIndex < potentials.Length; conIndex++)
                    {
                        if (potentials[conIndex] != null)
                        {
                            Type cand = constructorParams[conIndex][paramIndex];
                            int dist = SuperDistance(target, cand);
                            if (dist >= 0)
                            {
                                distances[conIndex] += dist;
                            }
                            else
                            {
                                potentials[conIndex] = null;
                                distances[conIndex] = -1;
                            }
                        }
                    }
                }

                this.constructor = (ConstructorInfo)Argmin(potentials, distances, 0);
                if (this.constructor == null)
                {
                    StringBuilder b = new StringBuilder();
                    b.Append(classname).Append(@"(");
                    foreach (Type c in params_renamed)
                    {
                        b.Append(c.FullName).Append(@", ");
                    }

                    string target = b.ToString().Substring(0, params_renamed.Length == 0 ? b.Length : b.Length - 2) + @")";
                    throw new ConstructorNotFoundException(@"No constructor found to match: " + target);
                }
            }

            public ClassFactory(string classname, params Type[] params_renamed)
            {
                Construct(classname, params_renamed);
            }

            public ClassFactory(string classname, params Object[] params_renamed)
            {
                Type[] classParams = new Type[params_renamed.Length];
                for (int i = 0; i < params_renamed.Length; i++)
                {
                    if (params_renamed[i] == null)
                        throw new ClassCreationException(@"Argument " + i + @" to class constructor is null");
                    classParams[i] = params_renamed[i].GetType();
                }

                Construct(classname, classParams);
            }

            public ClassFactory(string classname, params string[] params_renamed)
            {
                Type[] classParams = new Type[params_renamed.Length];
                for (int i = 0; i < params_renamed.Length; i++)
                {
                    classParams[i] = Type.GetType(params_renamed[i]);
                }

                Construct(classname, classParams);
            }

            public T CreateInstance(params Object[] params_renamed)
            {
                try
                {
                    //bool accessible = true;
                    //if (!constructor.IsPublic())
                    //{
                    //    accessible = false;
                    //    constructor.SetAccessible(true);
                    //}

                    T rtn = (T)constructor.Invoke(params_renamed);
                    //if (!accessible)
                    //{
                    //    constructor.SetAccessible(false);
                    //}

                    return rtn;
                }
                catch (Exception e)
                {
                    throw new ClassCreationException(@"MetaClass couldn't create " + constructor + @" with args " + string.Join(",", params_renamed), e);
                }
            }

            public string GetName()
            {
                return cl.FullName;
            }

            public override string ToString()
            {
                StringBuilder b = new StringBuilder();
                b.Append(cl.FullName).Append(@"(");
                foreach (Type cl2 in classParams)
                {
                    b.Append(@" ").Append(cl2.FullName).Append(@",");
                }

                b.Replace(b.Length - 1, b.Length, @" ");
                b.Append(@")");
                return b.ToString();
            }

            public override bool Equals(Object o)
            {
                if (o is ClassFactory<T>)
                {
                    ClassFactory<T> other = (ClassFactory<T>)o;
                    if (!this.cl.Equals(other.cl))
                        return false;
                    for (int i = 0; i < classParams.Length; i++)
                    {
                        if (!this.classParams[i].Equals(other.classParams[i]))
                            return false;
                    }

                    return true;
                }
                else
                {
                    return false;
                }
            }

            public override int GetHashCode()
            {
                return cl.GetHashCode();
            }
        }

        private string classname;
        public MetaClass(string classname)
        {
            this.classname = classname;
        }

        public MetaClass(Type classname)
        {
            this.classname = classname.FullName;
        }

        public virtual ClassFactory<E> CreateFactory<E>(params Type[] classes)
        {
            try
            {
                return new ClassFactory<E>(classname, classes);
            }
            catch (ClassCreationException e)
            {
                throw e;
            }
            catch (Exception e)
            {
                throw new ClassCreationException(e);
            }
        }

        public virtual ClassFactory<E> CreateFactory<E>(params string[] classes)
        {
            try
            {
                return new ClassFactory<E>(classname, classes);
            }
            catch (ClassCreationException e)
            {
                throw e;
            }
            catch (Exception e)
            {
                throw new ClassCreationException(e);
            }
        }

        public virtual ClassFactory<E> CreateFactory<E>(params Object[] objects)
        {
            try
            {
                return new ClassFactory<E>(classname, objects);
            }
            catch (ClassCreationException e)
            {
                throw e;
            }
            catch (Exception e)
            {
                throw new ClassCreationException(e);
            }
        }

        public virtual E CreateInstance<E>(params Object[] objects)
        {
            ClassFactory<E> fact = CreateFactory<E>(objects);
            return fact.CreateInstance(objects);
        }

        public virtual F CreateInstance<F>(Type type, params Object[] params_renamed)
        {
            Object obj = CreateInstance<F>(params_renamed);
            if (type.IsInstanceOfType(obj))
            {
                return (F)obj;
            }
            else
            {
                throw new ClassCreationException(@"Cannot cast " + classname + @" into " + type.FullName);
            }
        }

        public virtual bool CheckConstructor<E>(params Object[] params_renamed)
        {
            try
            {
                CreateInstance<E>(params_renamed);
                return true;
            }
            catch (ConstructorNotFoundException e)
            {
                return false;
            }
        }

        public override string ToString()
        {
            return classname;
        }

        public override bool Equals(Object o)
        {
            if (o is MetaClass)
            {
                return ((MetaClass)o).classname.Equals(this.classname);
            }

            return false;
        }

        public override int GetHashCode()
        {
            return classname.GetHashCode();
        }

        public static MetaClass Create(string classname)
        {
            return new MetaClass(classname);
        }

        public static MetaClass Create(Type clazz)
        {
            return new MetaClass(clazz);
        }

        private static Type Type2class(Type type)
        {
            if (type is Type)
            {
                return (Type)type;
            }
            //else if (type is ParameterizedType)
            //{
            //    return Type2class(((ParameterizedType)type).GetRawType());
            //}
            //else if (type is TypeVariable<?>)
            //{
            //    return Type2class(((TypeVariable<? >)type).GetBounds()[0]);
            //}
            //else if (type is WildcardType)
            //{
            //    return Type2class(((WildcardType)type).GetUpperBounds()[0]);
            //}
            else
            {
                throw new ArgumentException(@"Cannot convert type to class: " + type);
            }
        }

        private static String[] DecodeArray(string encoded)
        {
            char[] chars = encoded.Trim().ToCharArray();
            char quoteCloseChar = (char)0;
            List<StringBuilder> terms = new List<StringBuilder>();
            StringBuilder current = new StringBuilder();
            int start = 0;
            int end = chars.Length;
            if (chars[0] == '(')
            {
                start += 1;
                end -= 1;
                if (chars[end] != ')')
                    throw new ArgumentException(@"Unclosed paren in encoded array: " + encoded);
            }

            if (chars[0] == '[')
            {
                start += 1;
                end -= 1;
                if (chars[end] != ']')
                    throw new ArgumentException(@"Unclosed bracket in encoded array: " + encoded);
            }

            for (int i = start; i < end; i++)
            {
                if (chars[i] == '\\')
                {
                    if (i == chars.Length - 1)
                        throw new ArgumentException(@"Last character of encoded pair is escape character: " + encoded);
                    current.Append(chars[i + 1]);
                    i += 1;
                }
                else if (quoteCloseChar != 0)
                {
                    if (chars[i] == quoteCloseChar)
                    {
                        quoteCloseChar = (char)0;
                    }
                    else
                    {
                        current.Append(chars[i]);
                    }
                }
                else
                {
                    if (chars[i] == '"')
                    {
                        quoteCloseChar = '"';
                    }
                    else if (chars[i] == '\'')
                    {
                        quoteCloseChar = '\'';
                    }
                    else if (chars[i] == ',' || chars[i] == ' ' || chars[i] == '\t' || chars[i] == '\n')
                    {
                        if (current.Length > 0)
                        {
                            terms.Add(current);
                        }

                        current = new StringBuilder();
                    }
                    else
                    {
                        current.Append(chars[i]);
                    }
                }
            }

            if (current.Length > 0)
                terms.Add(current);
            String[] rtn = new string[terms.Size()];
            int i2 = 0;
            foreach (StringBuilder b in terms)
            {
                rtn[i2] = b.ToString().Trim();
                i2 += 1;
            }

            return rtn;
        }

        public static E Cast<E>(string value, Type type)
        {
            Type clazz = null;
            Type[] params_renamed = null;
            if (type is Type)
            {
                clazz = (Type)type;
            }
            //else if (type is ParameterizedType)
            //{
            //    ParameterizedType pt = (ParameterizedType)type;
            //    params_renamed = pt.GetActualTypeArguments();
            //    clazz = (Type)pt.GetRawType();
            //}
            else
            {
                clazz = Type2class(type);
                throw new ArgumentException(@"Cannot cast to type (unhandled type): " + type);
            }

            if (typeof(string).IsAssignableFrom(clazz))
            {
                return (E)(object)value;
            }
            else if (typeof(bool).IsAssignableFrom(clazz) || typeof(bool).IsAssignableFrom(clazz))
            {
                if (@"1".Equals(value))
                {
                    return (E)(object)true;
                }

                return (E)(object)bool.Parse(value);
            }
            else if (typeof(int).IsAssignableFrom(clazz) || typeof(int).IsAssignableFrom(clazz))
            {
                try
                {
                    return (E)(object)int.Parse(value);
                }
                catch (FormatException e)
                {
                    return (E)(object)(int)double.Parse(value);
                }
            }
            else if (typeof(long).IsAssignableFrom(clazz) || typeof(long).IsAssignableFrom(clazz))
            {
                try
                {
                    return (E)(object)long.Parse(value);
                }
                catch (FormatException e)
                {
                    return (E)(object)(long)double.Parse(value);
                }
            }
            else if (typeof(float).IsAssignableFrom(clazz) || typeof(float).IsAssignableFrom(clazz))
            {
                if (value == null)
                {
                    return (E)(object)(float.NaN);
                }

                return (E)(object)float.Parse(value);
            }
            else if (typeof(double).IsAssignableFrom(clazz) || typeof(double).IsAssignableFrom(clazz))
            {
                if (value == null)
                {
                    return (E)(object)double.NaN;
                }

                return (E)(object)double.Parse(value);
            }
            else if (typeof(short).IsAssignableFrom(clazz) || typeof(short).IsAssignableFrom(clazz))
            {
                try
                {
                    return (E)(object)short.Parse(value);
                }
                catch (FormatException e)
                {
                    return (E)(object)(short)double.Parse(value);
                }
            }
            else if (typeof(Byte).IsAssignableFrom(clazz) || typeof(byte).IsAssignableFrom(clazz))
            {
                try
                {
                    return (E)(object)byte.Parse(value);
                }
                catch (FormatException e)
                {
                    return (E)(object)(byte)double.Parse(value);
                }
            }
            else if (typeof(char).IsAssignableFrom(clazz) || typeof(char).IsAssignableFrom(clazz))
            {
                return (E)(object)(char)int.Parse(value);
            }
            else if (typeof(DateTime).IsAssignableFrom(clazz))
            {
                try
                {
                    return (E)(object)new DateTime(1970, 1, 1, 0, 0, 0).AddMilliseconds(long.Parse(value));
                }
                catch (FormatException e)
                {
                    return default(E);
                }
            }
            //else if (typeof (java.util.Calendar).IsAssignableFrom(clazz))
            //{
            //    try
            //    {
            //        Date d = new Date(Long.ParseLong(value));
            //        GregorianCalendar cal = new GregorianCalendar();
            //        cal.SetTime(d);
            //        return (E)cal;
            //    }
            //    catch (NumberFormatException e)
            //    {
            //        return null;
            //    }
            //}
            else if (typeof(FileInfo).IsAssignableFrom(clazz))
            {
                return (E)(object)new FileInfo(value);
            }
            else if (typeof(Type).IsAssignableFrom(clazz))
            {
                try
                {
                    return (E)(object)Type.GetType(value);
                }
                catch (TypeLoadException e)
                {
                    return default(E);
                }
            }
            else if (clazz.IsArray)
            {
                if (value == null)
                {
                    return default(E);
                }

                Type subType = clazz.GetElementType();
                String[] strings = DecodeArray(value);
                Object[] array = (Object[])Array.CreateInstance(clazz.GetElementType(), strings.Length);
                for (int i = 0; i < strings.Length; i++)
                {
                    array[i] = Cast<E>(strings[i], subType);
                }

                return (E)(object)array;
            }
            else if (clazz.IsEnum)
            {
                Type c = (Type)clazz;
                if (value == null)
                {
                    return default(E);
                }

                if (value[0] == '"')
                    value = value.Substring(1);
                if (value[value.Length - 1] == '"')
                    value = value.Substring(0, value.Length - 1);
                try
                {
                    return (E)Enum.Parse(c, value);
                }
                catch (Exception e)
                {
                    try
                    {
                        return (E)Enum.Parse(c, value.ToLower());
                    }
                    catch (Exception e2)
                    {
                        try
                        {
                            return (E)Enum.Parse(c, value.ToUpper());
                        }
                        catch (Exception e3)
                        {
                            return (E)Enum.Parse(c, (char.IsUpper(value[0]) ? char.ToLower(value[0]) : char.ToUpper(value[0])) + value.Substring(1));
                        }
                    }
                }
            }
            else
            {
                try
                {
                    MethodInfo decode = clazz.GetMethod("FromString", new[] { typeof(string) });
                    return (E)(object)decode.Invoke(MetaClass.Create(clazz), new[] { value });
                }
                catch (MissingMethodException e)
                {
                }
                catch (TargetInvocationException e)
                {
                }
                catch (MethodAccessException e)
                {
                }
                catch (InvalidCastException e)
                {
                }

                return default(E);
            }
        }

        private static E Argmin<E>(E[] elems, int[] scores, int atLeast)
        {
            int argmin = Argmin<E>(scores, atLeast);
            return argmin >= 0 ? (E)elems[argmin] : default(E);
        }

        private static int Argmin<E>(int[] scores, int atLeast)
        {
            int min = int.MaxValue;
            int argmin = -1;
            for (int i = 0; i < scores.Length; i++)
            {
                if (scores[i] < min && scores[i] >= atLeast)
                {
                    min = scores[i];
                    argmin = i;
                }
            }

            return argmin;
        }
    }
}
