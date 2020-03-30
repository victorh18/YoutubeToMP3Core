import React, {useState, useEffect, useCallback} from 'react';
import './App.css';
import ytService from './services/youtubeService';
import {Row, Col, Input, Typography, Select, Spin, Menu, Button, Space} from 'antd';
import { LoadingOutlined, VideoCameraOutlined, UnorderedListOutlined } from '@ant-design/icons';
import VideoPreview from './components/VideoPreview';
import util from './util/util';
const {Title} = Typography;
const {Option} = Select;
const {debounce} = util;
const {SubMenu} = Menu;

const antIcon = <LoadingOutlined style={{ fontSize: 24 }} spin />;

const FormatSelect = (props) => {
  return (<Select defaultValue={2} onChange={props.onChange} value={props.value}>
            <Option value={1}>Video</Option>
            <Option value={2}>Audio</Option>
          </Select>)
}

function App() {
  const [url, setUrl] = useState('');
  const [format, setFormat] = useState(2);
  const [meta, setMeta] = useState(null);
  const [loadingMeta, setLoadingMeta] = useState(false);
  const [currentMenu, setCurrentMenu] = useState('video');

  const dGetMeta = useCallback(debounce(getMeta, 1000), []);

  function handleMenuClick(e) {
    setCurrentMenu(e.key);
  }
  
  function getMeta(urlp) {
    setLoadingMeta(true);

    ytService.getMetadata(urlp).then(data => {
      setMeta(data);
    }).finally(() => {
      setLoadingMeta(false);
    });
  };

  function downloadVid(urlp) {
    ytService.download(urlp, format);
  }

  useEffect(() => {
    dGetMeta(url);
  }, [url]);

  return (
    <div className="App">
      <Menu onClick={handleMenuClick} selectedKeys={[currentMenu]} mode="horizontal">
        <Menu.Item key={'video'}><VideoCameraOutlined /> Video</Menu.Item>
        <Menu.Item key={'playlist'}><UnorderedListOutlined /> Playlist</Menu.Item>
      </Menu>
      {currentMenu == 'video' && 
        <React.Fragment>
          <Row>
          <Col span={20} offset={2}>

            {!loadingMeta && meta && 
              <Row style={{marginTop: '10px', marginBottom: '10px'}}>
                <Col span={12} offset={6}>
                  <VideoPreview Meta={meta}></VideoPreview>
                </Col>
              </Row>
            }
            
            {!meta && 
              <Row style={{marginTop: '10px', marginBottom: '10px'}}>
                <Col span={20} offset={2}>
                  <Title >Inserta el URL del video</Title>
                </Col>
              </Row>
            }

            {loadingMeta && 
              <Row style={{marginTop: '10px', marginBottom: '10px'}}>
                <Col span={20} offset={2}>
                  <Spin size='large' indicator={antIcon} />
                </Col>
              </Row>
            }

            

            {meta && 
            <Row style={{marginTop: '10px', marginBottom: '10px'}}>
              <Col span={20} offset={2}>
                <Button onClick={() => {downloadVid(url)}} size={'large'} type="primary">Descargar</Button>  
              </Col>
            </Row>
            }

            <Input size="large" onChange={({target: {value}}) => setUrl(value)} value={url} 
                    addonAfter={<FormatSelect value={format} onChange={props => {setFormat(props)}}></FormatSelect>} 
                    placeholder="URL del video"></Input>
          </Col>
        </Row>

        

        
        </React.Fragment>
      }

      {currentMenu == 'playlist' && 
        <React.Fragment>

        </React.Fragment>
      }


      
    </div>
  );
}

export default App;
