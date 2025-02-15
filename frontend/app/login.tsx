import { FBAuth } from '@/firebase/auth';
import { Redirect, useRouter } from 'expo-router';
import { useEffect, useState } from 'react';
import {
  View,
  Text,
  TextInput,
  TouchableOpacity,
  StyleSheet,
  SafeAreaView,
  ScrollView,
} from 'react-native';
import { styles } from './styles/styles';
import TextStyles from './styles/textStyles';

const LoginScreen = () => {
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [name, setName] = useState("")
  const [signUpToggle, setSignUpToggle] = useState(false)
  const [error, setError] = useState("")
  const [nameError, setNameError] = useState("")
  const [emailError, setEmailError] = useState("")
  const [passwordError, setPasswordError] = useState("")
  const [confirmPasswordError, setConfirmPasswordError] = useState("")
  const [confirmPassword, setConfirmPassword] = useState("")


  const [redirect, setRedirect] = useState("" as "/home")

  const handleLogin = () => {
    clearErrors()
    if (signUpToggle && ValidName() && passwordsMatch()) {
        FBAuth.createUser(email, password, name).then(() => {
            setRedirect("/home")
        }).catch((error) => handleError(error))
    } else if (!signUpToggle) {
        FBAuth.authenticate(email, password).then(() => {
          setRedirect("/home")
        }).catch((error) => handleError(error))
    }
    console.log('Login attempted with:', email, password);
  };

  const clearErrors = () => {
    setEmailError("")
    setPasswordError("")
    setNameError("")
    setError("")
    setConfirmPasswordError("")
  }

  const clearFields = () => {
    setPassword("")
    setEmail("")
    setName("")
  }

  const handleError = (error: string) => {
    if (error.includes("invalid-email")) {
      setEmailError("Invalid Email")
    } else if (error.includes("invalid-credential")) {
      setPasswordError("Invalid Password")
    } else if (error.includes("weak-password")) {
      setPasswordError("Please choose a stronger password.")
    } else if (error.includes("network-request-failed")) {
      setError("Failed to connect to server. Check internet connection.")
    } else {
      setError(error)
    } 
  }

  const ValidName = (): boolean => {
    let ret = true;
    if (name.length < 5) {
      setNameError("Name is too short.")
      ret = false
    } else if (!name.includes(" ")) {
      setNameError("Please include last name or initial.")
      ret = false
    }
    return ret
  }

  const passwordsMatch = () => {
    if (confirmPassword != password) {
      setConfirmPasswordError("Passwords do not match.")
      return false
    }
    return true
  }


  return (redirect.length == 0 ?
    <SafeAreaView style={styles.container}>
        <ScrollView style={{width: "60%", marginBottom: 30}}
        showsVerticalScrollIndicator={false}>
            <Text style={{...TextStyles.h1, color: "white", marginTop: 40}}>{signUpToggle ? "Join the" : "Welcome to your"} playground.</Text>
            <Text style={{...TextStyles.subtitle, marginTop: 0, fontSize: 20, color: "white"}}>Huro VR</Text>
            <View style={{ height: 1, backgroundColor: '#ccc', width: '90%', marginVertical: 10, marginBottom: 20 }} />

            <View style={styles.formContainer}>
                {signUpToggle && <View style={styles.inputContainer}>
                    <Text style={styles.label}>Name</Text>
                    <TextInput
                        style={styles.input}
                        placeholder="Enter your name"
                        placeholderTextColor="#666"
                        value={name}
                        onChangeText={setName}
                        autoCapitalize="words"
                    />
                    <Text style={styles.errorText}>{nameError}</Text>
                </View>}
                <View style={styles.inputContainer}>
                    <Text style={styles.label}>Email</Text>
                    <TextInput
                        style={styles.input}
                        placeholder="Enter your email"
                        placeholderTextColor="#666"
                        value={email}
                        onChangeText={setEmail}
                        keyboardType="email-address"
                        autoCapitalize="none"
                    />
                    <Text style={styles.errorText}>{emailError}</Text>
                </View>
                

                <View style={styles.inputContainer}>
                    <Text style={styles.label}>Password</Text>
                    <TextInput
                        style={styles.input}
                        placeholder="Enter your password"
                        placeholderTextColor="#666"
                        value={password}
                        onChangeText={setPassword}
                        onSubmitEditing={() => {
                          handleLogin()
                        }}
                        secureTextEntry
                    />
                    <Text style={styles.errorText}>{passwordError}</Text>
                </View>

                {signUpToggle && <View style={styles.inputContainer}>
                    <Text style={styles.label}>Confirm Password</Text>
                    <TextInput
                        style={styles.input}
                        placeholder="Confirm Password"
                        placeholderTextColor="#666"
                        onChangeText={setConfirmPassword}
                        value={confirmPassword}
                        onSubmitEditing={handleLogin}
                        secureTextEntry
                    />
                    <Text style={styles.errorText}>{confirmPasswordError}</Text>
                </View>}

                {error.length > 0 && <Text style={styles.errorText}>{error}</Text>}

                <TouchableOpacity 
                style={styles.loginButton}
                onPress={handleLogin}
                >
                <Text style={styles.loginButtonText}>{signUpToggle ? "Sign Up" : "Login"}</Text>
                </TouchableOpacity>

                <TouchableOpacity 
                style={styles.signUpToggle}
                onPress={() => {
                    clearErrors()
                    clearFields()
                    setSignUpToggle(!signUpToggle)
                }}
                >
                <Text style={styles.signUpText}>{!signUpToggle ? "Sign Up" : "Login"}</Text>
                </TouchableOpacity>
            </View>
      </ScrollView>
      {}
    </SafeAreaView>
  : <Redirect href={redirect}/>);
};


export default LoginScreen;