import React from "react";
import { View, ViewProps } from "react-native";

interface ConditionalViewProps extends ViewProps {
  isVisible: boolean;
}

const ConditionalView: React.FC<ConditionalViewProps> = ({ isVisible, children, ...rest }) => {
  if (!isVisible) return null;
  return <View {...rest}>{children}</View>;
};

export default ConditionalView;
