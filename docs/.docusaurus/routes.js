import React from 'react';
import ComponentCreator from '@docusaurus/ComponentCreator';

export default [
  {
    path: '/markdown-page',
    component: ComponentCreator('/markdown-page', '3d7'),
    exact: true
  },
  {
    path: '/',
    component: ComponentCreator('/', 'e5f'),
    exact: true
  },
  {
    path: '/',
    component: ComponentCreator('/', '94a'),
    routes: [
      {
        path: '/',
        component: ComponentCreator('/', 'b73'),
        routes: [
          {
            path: '/',
            component: ComponentCreator('/', '616'),
            routes: [
              {
                path: '/backend-dotnet',
                component: ComponentCreator('/backend-dotnet', '44e'),
                exact: true,
                sidebar: "tutorialSidebar"
              },
              {
                path: '/deployment',
                component: ComponentCreator('/deployment', 'a8a'),
                exact: true,
                sidebar: "tutorialSidebar"
              },
              {
                path: '/frontend-vite',
                component: ComponentCreator('/frontend-vite', '3cf'),
                exact: true,
                sidebar: "tutorialSidebar"
              },
              {
                path: '/google-cloud-setup',
                component: ComponentCreator('/google-cloud-setup', '672'),
                exact: true,
                sidebar: "tutorialSidebar"
              },
              {
                path: '/overview',
                component: ComponentCreator('/overview', 'aff'),
                exact: true,
                sidebar: "tutorialSidebar"
              },
              {
                path: '/running-locally',
                component: ComponentCreator('/running-locally', 'f41'),
                exact: true,
                sidebar: "tutorialSidebar"
              },
              {
                path: '/troubleshooting',
                component: ComponentCreator('/troubleshooting', '810'),
                exact: true,
                sidebar: "tutorialSidebar"
              }
            ]
          }
        ]
      }
    ]
  },
  {
    path: '*',
    component: ComponentCreator('*'),
  },
];
