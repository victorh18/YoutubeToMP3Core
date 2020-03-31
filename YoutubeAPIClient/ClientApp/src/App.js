import React, {useState, useEffect, useCallback} from 'react';
import './App.css';
import ytService from './services/youtubeService';
import {Row, Col, Input, Typography, Select, Spin, Menu, Button, Space} from 'antd';
import { LoadingOutlined, VideoCameraOutlined, UnorderedListOutlined } from '@ant-design/icons';
import VideoPreview from './components/VideoPreview';
import util from './util/util';
import VideoDownloader from './components/VideoDownloader';
import PlaylistDownloader from './components/PlaylistDownloader';
import FormatSelect from './components/FormatSelect';
const {Title} = Typography;
const {Option} = Select;
const {debounce, isURL} = util;
const {SubMenu} = Menu;

const antIcon = <LoadingOutlined style={{ fontSize: 24 }} spin />;

function App() {
  const [currentMenu, setCurrentMenu] = useState('video');

  function handleMenuClick(e) {
    setCurrentMenu(e.key);
  }

  return (
    <div className="App">
      <Menu onClick={handleMenuClick} selectedKeys={[currentMenu]} mode="horizontal">
        <Menu.Item key={'video'}><VideoCameraOutlined /> Video</Menu.Item>
        <Menu.Item key={'playlist'}><UnorderedListOutlined /> Playlist</Menu.Item>
      </Menu>
      {currentMenu == 'video' && 
        <VideoDownloader>
        </VideoDownloader>
      }
      {currentMenu == 'playlist' && 
        <PlaylistDownloader>
        </PlaylistDownloader>
      }
    </div>
  );
}

export default App;
