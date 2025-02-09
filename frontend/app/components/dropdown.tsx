import React, { useState } from 'react';
import {
  View,
  Text,
  TouchableOpacity,
  Animated,
  StyleSheet,
  LayoutAnimation,
  Platform,
  UIManager,
} from 'react-native';
import { ViewStyle, TextStyle } from 'react-native';


// Enable LayoutAnimation for Android
if (Platform.OS === 'android') {
  if (UIManager.setLayoutAnimationEnabledExperimental) {
    UIManager.setLayoutAnimationEnabledExperimental(true);
  }
}

const DropdownMenu = ({ 
  options, 
  defaultValue = 'Select an option',
  onSelect,
  containerStyle,
  dropdownStyle,
  optionStyle,
  textStyle,
}: DropdownProps) => {
  const [isOpen, setIsOpen] = useState(false);
  const [selectedOption, setSelectedOption] = useState(defaultValue);
  const rotateAnimation = new Animated.Value(0);

  const toggleDropdown = () => {
    LayoutAnimation.configureNext(LayoutAnimation.Presets.easeInEaseOut);
    setIsOpen(!isOpen);
    Animated.timing(rotateAnimation, {
      toValue: isOpen ? 0 : 1,
      duration: 300,
      useNativeDriver: true,
    }).start();
  };

  const handleSelect = (option: string) => {
    setSelectedOption(option);
    setIsOpen(false);
    onSelect && onSelect(option);
    Animated.timing(rotateAnimation, {
      toValue: 0,
      duration: 300,
      useNativeDriver: true,
    }).start();
  };

  const rotateInterpolate = rotateAnimation.interpolate({
    inputRange: [0, 1],
    outputRange: ['0deg', '180deg'],
  });

  const animatedStyle = {
    transform: [{ rotate: rotateInterpolate }],
  };

  return (
    <View style={[styles.container, containerStyle]}>
      <TouchableOpacity 
        style={[styles.dropdownButton, dropdownStyle]} 
        onPress={toggleDropdown}
        activeOpacity={0.7}
      >
        <Text style={[styles.selectedText, textStyle]}>{selectedOption}</Text>
        <Animated.Text style={[styles.arrow, animatedStyle]}>▼</Animated.Text>
      </TouchableOpacity>
      
      {isOpen && (
        <View style={[styles.optionsContainer, dropdownStyle]}>
          {options.map((option, index) => (
            <TouchableOpacity
              key={index}
              style={[styles.option, optionStyle]}
              onPress={() => handleSelect(option)}
              activeOpacity={0.7}
            >
              <Text style={[styles.optionText, textStyle]}>{option}</Text>
            </TouchableOpacity>
          ))}
        </View>
      )}
    </View>
  );
};

interface DropdownProps {
    /**
     * Array of options to be displayed in the dropdown
     */
    options: string[];
  
    /**
     * Default text to show when no option is selected
     * @default 'Select an option'
     */
    defaultValue?: string;
  
    /**
     * Callback function triggered when an option is selected
     * @param selectedOption The selected option value
     */
    onSelect?: (selectedOption: string) => void;
  
    /**
     * Style object for the main container view
     */
    containerStyle?: ViewStyle;
  
    /**
     * Style object for the dropdown button and options container
     */
    dropdownStyle?: ViewStyle;
  
    /**
     * Style object for individual option items
     */
    optionStyle?: ViewStyle;
  
    /**
     * Style object for text elements within the dropdown
     */
    textStyle?: TextStyle;
  }

const styles = StyleSheet.create({
  container: {
    position: 'relative',
    zIndex: 1000,
    width: '100%',
    maxWidth: 300,
  },
  dropdownButton: {
    flexDirection: 'row',
    justifyContent: 'space-between',
    alignItems: 'center',
    padding: 12,
    backgroundColor: '#fff',
    borderRadius: 8,
    borderWidth: 1,
    borderColor: '#ddd',
  },
  selectedText: {
    fontSize: 16,
    color: '#333',
  },
  arrow: {
    fontSize: 12,
    color: '#666',
  },
  optionsContainer: {
    position: 'absolute',
    top: '100%',
    left: 0,
    right: 0,
    backgroundColor: '#fff',
    borderRadius: 8,
    borderWidth: 1,
    borderColor: '#ddd',
    marginTop: 4,
    shadowColor: '#000',
    shadowOffset: {
      width: 0,
      height: 2,
    },
    shadowOpacity: 0.25,
    shadowRadius: 3.84,
    elevation: 5,
  },
  option: {
    padding: 12,
    borderBottomWidth: 1,
    borderBottomColor: '#eee',
  },
  optionText: {
    fontSize: 16,
    color: '#333',
  },
});

export default DropdownMenu;