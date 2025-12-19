import type {ReactNode} from 'react';
import clsx from 'clsx';
import Link from '@docusaurus/Link';
import useDocusaurusContext from '@docusaurus/useDocusaurusContext';
import Layout from '@theme/Layout';
import Heading from '@theme/Heading';

import styles from './index.module.css';

function HomepageHeader() {
  const {siteConfig} = useDocusaurusContext();
  return (
    <header className={clsx('hero hero--primary', styles.heroBanner)}>
      <div className="container">
        <p className={styles.kicker}>Google OAuth · ASP.NET Core · React</p>
        <Heading as="h1" className="hero__title">
          {siteConfig.title}
        </Heading>
        <p className="hero__subtitle">{siteConfig.tagline}</p>
        <div className={styles.ctas}>
          <Link className="button button--secondary button--lg" to="/backend-dotnet">
            Start with the backend
          </Link>
          <Link className="button button--outline button--lg" to="/google-cloud-setup">
            Configure Google
          </Link>
        </div>
      </div>
    </header>
  );
}

const Steps = [
  {
    title: 'Create the OAuth client',
    description: 'Add localhost origins, the callback path, and copy your client ID/secret from Google Cloud.',
    to: '/google-cloud-setup',
  },
  {
    title: 'Wire up the backend',
    description:
      'Use cookie auth + Google challenge in ASP.NET Core; keep secrets in user-secrets; expose /api/auth/login, /logout, /me.',
    to: '/backend-dotnet',
  },
  {
    title: 'Connect the frontend',
    description: 'Use Vite proxy to call /api with credentials included; show the signed-in user or redirect to login.',
    to: '/frontend-vite',
  },
];

function StepCard({title, description, to}: {title: string; description: string; to: string}) {
  return (
    <Link className={styles.card} to={to}>
      <div className={styles.cardBody}>
        <p className={styles.cardTitle}>{title}</p>
        <p className={styles.cardText}>{description}</p>
        <span className={styles.cardLink}>Open section →</span>
      </div>
    </Link>
  );
}

export default function Home(): ReactNode {
  const {siteConfig} = useDocusaurusContext();
  return (
    <Layout title={siteConfig.title} description={siteConfig.tagline}>
      <HomepageHeader />
      <main className={styles.main}>{Steps.map((step) => StepCard(step))}</main>
    </Layout>
  );
}
