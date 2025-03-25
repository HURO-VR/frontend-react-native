import { UserMetaData } from '@/firebase/models';
import { User } from 'firebase/auth';
import React, { useState, useEffect, useRef } from 'react';
import {
  View,
  Text,
  TextInput,
  TouchableOpacity,
  FlatList,
  StyleSheet,
  Keyboard,
  StyleProp,
  ViewStyle,
} from 'react-native';
import { styles } from '../styles/styles';
import { FBFirestore } from '@/firebase/firestore';


interface Props {
    selectedUsers: UserMetaData[],
    setSelectedUsers: any
    dropdownStyle?: StyleProp<ViewStyle>
}

const UserSelector = ({selectedUsers, setSelectedUsers, dropdownStyle}: Props) => {
  const [inputValue, setInputValue] = useState('');
  const [suggestions, setSuggestions] = useState([] as UserMetaData[]);
  const [isFocused, setIsFocused] = useState(false);
  const inputRef = useRef<TextInput>(null);


  useEffect(() => {
    if (inputValue.trim()) {
      // TODO: Currently case sensitive.
      FBFirestore.listUsers(inputValue).then((data) => {
        setSuggestions(data)
      }).catch((e) => {
        console.error(e)
        setSuggestions([]);
      })
    } else {
      setSuggestions([]);
    }
  }, [inputValue, selectedUsers]);

  const handleSelectRecipient = (recipient: UserMetaData) => {
    setSelectedUsers([...selectedUsers, recipient]);
    setInputValue('');
    setSuggestions([]);
  };

  const handleRemoveRecipient = (recipientId: string) => {
    setSelectedUsers(selectedUsers.filter((r) => r.uid !== recipientId));
  };

  const renderRecipientChip = ({ item }: any) => (
    <View style={styles.chipContainer}>
      <Text style={styles.chipText}>{item.name}</Text>
      <TouchableOpacity
        onPress={() => handleRemoveRecipient(item.uid)}
        style={styles.removeButton}
      >
        <Text style={styles.removeButtonText}>×</Text>
      </TouchableOpacity>
    </View>
  );

  const renderSuggestion = ({ item }: any) => (
    <TouchableOpacity
      style={styles.suggestionItem}
      onPress={() => handleSelectRecipient(item)}
    >
      <Text style={styles.suggestionName}>{item.name}</Text>
      <Text style={styles.suggestionEmail}>{item.email}</Text>
    </TouchableOpacity>
  );

  return (
    <View style={{flex: 1}}>
      <View style={{}}>
        <FlatList
          data={selectedUsers}
          renderItem={renderRecipientChip}
          keyExtractor={(item) => item.uid}
          horizontal
          showsHorizontalScrollIndicator={false}
          contentContainerStyle={styles.recipientsList}
        />
        <TextInput
          style={styles.input}
          value={inputValue}
          //ref={inputRef}
          onChangeText={setInputValue}
          onFocus={() => setIsFocused(true)}
          placeholder={selectedUsers.length === 0 ? "Enter email addresses..." : ""}
          placeholderTextColor="#666"
          onSubmitEditing={(event) => {
            if (suggestions.length > 0) {
                handleSelectRecipient(suggestions[0])
                event.target.focus();
            }
          }}
        />
      </View>

      {isFocused && suggestions.length > 0 && (
        <FlatList
          data={suggestions}
          renderItem={renderSuggestion}
          keyExtractor={(item) => item.uid}
          style={[styles.suggestionsList, dropdownStyle]}
          keyboardShouldPersistTaps="never"
        />
      )}
    </View>
  );
};


export default UserSelector;