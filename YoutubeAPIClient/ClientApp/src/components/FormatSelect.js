import React from 'react';
import {Select} from 'antd';
const {Option} = Select;

export default function FormatSelec(props) {
    return (<Select defaultValue={2} onChange={props.onChange} value={props.value}>
        <Option value={1}>Video</Option>
        <Option value={2}>Audio</Option>
      </Select>)
}